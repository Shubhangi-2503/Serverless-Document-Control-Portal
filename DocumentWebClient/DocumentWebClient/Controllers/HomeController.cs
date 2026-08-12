using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace DocumentWebClient.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly string _storageConnectionString;
        private const string ContainerName = "user-documents";

        public HomeController(IHttpClientFactory clientFactory, IConfiguration configuration)
        {
            _clientFactory = clientFactory;
            _storageConnectionString = configuration.GetConnectionString("AzureBlobStorage");
        }

        public async Task<IActionResult> Index()
        {
            var fileList = new List<string>();
            try
            {
                var options = new BlobClientOptions();
                options.Diagnostics.IsLoggingEnabled = false;
                var blobServiceClient = new BlobServiceClient(_storageConnectionString, options);
                var containerClient = blobServiceClient.GetBlobContainerClient(ContainerName);

                if (await containerClient.ExistsAsync())
                {
                    await foreach (BlobItem blobItem in containerClient.GetBlobsAsync())
                    {
                        fileList.Add(blobItem.Name);
                    }
                }
            }
            catch { }
            return View(fileList);
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile documentFile)
        {
            if (documentFile == null || documentFile.Length == 0)
            {
                TempData["Message"] = "Please select a valid file first.";
                TempData["IsSuccess"] = false;
                return RedirectToAction("Index");
            }

            try
            {
                var client = _clientFactory.CreateClient("AzureFunctionClient");
                using var content = new MultipartFormDataContent();
                using var fileStream = documentFile.OpenReadStream();
                using var streamContent = new StreamContent(fileStream);

                streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(documentFile.ContentType);
                content.Add(streamContent, "document", documentFile.FileName);

                // Send the file to the Function App backend
                HttpResponseMessage response = await client.PostAsync("api/UploadDocument", content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["Message"] = "Success! File transferred through the pipeline.";
                    TempData["IsSuccess"] = true;
                }
                else
                {
                    // This captures if the backend rejected the payload
                    string backendError = await response.Content.ReadAsStringAsync();
                    TempData["Message"] = $"Backend Failure ({response.StatusCode}): {backendError}";
                    TempData["IsSuccess"] = false;
                }
            }
            catch (Exception ex)
            {
                // This captures if the network connection failed entirely (e.g. bad port number)
                TempData["Message"] = $"Network Client Routing Error: {ex.Message}";
                TempData["IsSuccess"] = false;
            }

            return RedirectToAction("Index");
        }
    }
}

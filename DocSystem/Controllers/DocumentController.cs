using System.Reflection.Metadata;
using Business.Services.Documents;
using Business.Services.Students;
using DataAccess.Entities;
using DocSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Document = DataAccess.Entities.Document;

namespace DocSystem.Controllers
{
    public class DocumentController : Controller
    {
        private readonly IDocumentService documentService;
        private readonly IStudentService studentService;

        public DocumentController(IDocumentService documentService , IStudentService studentService)
        {
            this.documentService = documentService;
            this.studentService = studentService;
        }


        public async Task<IActionResult> Index()
        {
            ViewBag.Documents = await documentService.GetDocuments();
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Archive()
        {
            ViewBag.Students = await studentService.GetStudents();

            var document = new Document();
            return View(document);
        }

        [HttpGet]
        public IActionResult QrValidation()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Print(int Id)
        {
            try
            {
               
                var file = await documentService.Print(Id);
                return File(file, "application/pdf");
                
            }
            catch
            {
                return BadRequest();
            }
        }






        [HttpPost]
        public async Task<IActionResult> Archive(Document document)
        {
            try
            {
                await documentService.Archive(document);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return Json( ex.Message );
            }
        }

        [HttpPost]
        public async Task<IActionResult> CheckValid(string refNumber)
        {
            try
            {
               var valid =  await documentService.CheckValid(refNumber);
                if(valid)
                return Ok("the document ok");
                else
                    return BadRequest("the document not real");
            }
            catch
            {
                return BadRequest("the document not real");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int Id)
        {
            try
            {
                await documentService.Remove(Id);
                return RedirectToAction("Index");
            }
            catch {
                return BadRequest();
            }

        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RpgMvc.Models;

namespace RpgMvc.Controllers
{
    public class DisputasController : Controller
    {
        public string uriBase = "http://diegorpg.somee.com/Disputas";


        [HttpGet]
        public async Task<ActionResult> IndexAsync()
        {
            try
            {
                HttpClient httpClient = new HttpClient();
                string token = HttpContext.Session.GetString("SessionTokenUsuario");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                string uriBuscaPersonagens = "http://diegorpg.somee.com/Personagens/GetAll";
                HttpResponseMessage response = await httpClient.GetAsync(uriBuscaPersonagens);
                string serialized = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    List<PersonagemViewModel> listarPersonagens = await Task.Run(() =>
                        JsonConvert.DeserializeObject<List<PersonagemViewModel>>(serialized));

                    ViewBag.ListarAtacantes = listarPersonagens;
                    ViewBag.ListarOponentes = listarPersonagens;
                    return View();
                }
                else
                    throw new System.Exception(serialized);
            }
            catch (System.Exception ex)
            {
                TempData["MensagemErro"] = ex.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<ActionResult> IndexAsync(DisputaViewModel disputa)
        {
            try
            {
                HttpClient httpClient = new HttpClient();
                string uriComplementar = "Arma";

                var content = new StringContent(JsonConvert.SerializeObject(disputa));
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                HttpResponseMessage response = await httpClient.PostAsync(uriBase + uriComplementar, content);
                string serialized = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    disputa = await Task.Run(() => JsonConvert.DeserializeObject<DisputaViewModel>(serialized));
                    TempData["Mensagem"] = disputa.Narracao;
                    return RedirectToAction("Index", "Personagens");
                }
                else
                    throw new System.Exception(serialized);
            }
            catch (System.Exception ex)
            {
                TempData["MensagemErro"] = ex.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        public async Task<ActionResult> IndexHabilidadesAsync()
        {
            try
            {
                HttpClient httpClient = new HttpClient();
                string token = HttpContext.Session.GetString("SessionTokenUsuario");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                string uriBuscaPersonagens = "http://diegorpg.somee.com/Personagens/GetAll";
                HttpResponseMessage response = await httpClient.GetAsync(uriBuscaPersonagens);
                string serialized = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    List<PersonagemViewModel> listarPersonagens = await Task.Run(() =>
                        JsonConvert.DeserializeObject<List<PersonagemViewModel>>(serialized));

                    ViewBag.ListarAtacantes = listarPersonagens;
                    ViewBag.ListarOponentes = listarPersonagens;
                }
                else
                    throw new System.Exception(serialized);

                string uriBuscaHabilidades = "http://diegorpg.somee.com/PersonagemHabilidades/GetHabilidades";
                response = await httpClient.GetAsync(uriBuscaHabilidades);
                serialized = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    List<HabilidadeViewModel> listaHabilidades = await Task.Run(() =>
                        JsonConvert.DeserializeObject<List<HabilidadeViewModel>>(serialized));
                    ViewBag.listaHabilidades = listaHabilidades;
                }
                else
                    throw new System.Exception(serialized);
                return View("IndexHabilidades");
            }
            catch (System.Exception)
            {

                throw;
            }
        }


        [HttpPost]
        public async Task<ActionResult> IndexHabilidadesAsync(DisputaViewModel disputa)
        {
            try
            {
                HttpClient httpClient = new HttpClient();
                string uriComplementar = "Habilidade";
                var content = new StringContent(JsonConvert.SerializeObject(disputa));
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                HttpResponseMessage response = await httpClient.PostAsync(uriBase + uriComplementar, content);
                string serialized = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    disputa = await Task.Run(() =>
                    JsonConvert.DeserializeObject<DisputaViewModel>(serialized));
                    TempData["Mensagem"] = disputa.Narracao;
                    return RedirectToAction("Index", "Personagens");
                }
                else
                    throw new System.Exception(serialized);
            }
            catch (System.Exception ex)
            {
                TempData["MensagemErro"] = ex.Message;
                return RedirectToAction("Index");
            }
        }
        [HttpGet]
        public async Task<ActionResult> IndexDisputasAsync()
        {
            try
            {
                string uriComplementar = "Listar";
                HttpClient httpClient = new HttpClient();
                string token = HttpContext.Session.GetString("SessionTokenUsuario");
                httpClient.DefaultRequestHeaders.Authorization = new
                AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage response = await httpClient.GetAsync(uriBase +
                uriComplementar);
                string serialized = await response.Content.ReadAsStringAsync();
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    List<DisputaViewModel> lista = await Task.Run(() =>
                    JsonConvert.DeserializeObject<List<DisputaViewModel>>(serialized));
                    return View(lista);
                }
                else
                    throw new System.Exception(serialized);
            }
            catch (System.Exception ex)
            {
                TempData["MensagemErro"] = ex.Message;
                return RedirectToAction("Index");
            }
        }
        [HttpGet]
        public async Task<ActionResult> ApagarDisputasAsync()
        {
            try
            {
                HttpClient httpClient = new HttpClient();
                string token = HttpContext.Session.GetString("SessionTokenUsuario");
                httpClient.DefaultRequestHeaders.Authorization = new
                AuthenticationHeaderValue("Bearer", token);
                string uriComplementar = "ApagarDisputas";
                HttpResponseMessage response = await httpClient.DeleteAsync(uriBase +
                uriComplementar);
                string serialized = await response.Content.ReadAsStringAsync();
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    TempData["Mensagem"] = "Disputas apagadas com sucessso.";
                else
                    throw new System.Exception(serialized);
            }
            catch (System.Exception ex)
            {
                TempData["MensagemErro"] = ex.Message;
            }
            return RedirectToAction("IndexDisputas", "Disputas");
        }
    }
}
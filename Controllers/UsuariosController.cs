using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Headers;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RpgMvc.Models;

namespace RpgMvc.Controllers
{
    public class UsuariosController : Controller
    {
        public string uriBase = "http://www.diegorpg.somee.com/Usuarios/";



        [HttpGet]
        public ActionResult Index()
        {
            return View("CadastrarUsuario");
        }

        [HttpPost]
        public async Task<ActionResult> RegistrarAsync(UsuarioViewModel u)
        {
            try
            {
                HttpClient httpClient = new HttpClient();
                string uriComplementar = "Registrar";

                var content = new StringContent(JsonConvert.SerializeObject(u));
                content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                HttpResponseMessage response = await httpClient.PostAsync(uriBase + uriComplementar, content);

                string serialized = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    TempData["Mensagem"] =
                        string.Format("Usuário {0} Registrado com sucesso! Faça o login para acessar.", u.Username);
                    return View("AutenticarUsuario");
                }
                else
                {
                    throw new System.Exception(serialized);
                }
            }
            catch (System.Exception ex)
            {
                TempData["MensagemErro"] = ex.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        public ActionResult IndexLogin()
        {
            return View("AutenticarUsuario");
        }

        [HttpPost]
        public async Task<ActionResult> AutenticarAsync(UsuarioViewModel u)
        {
            try
            {
                HttpClient httpClient = new HttpClient();
                string uriComplementar = "Autenticar";

                var content = new StringContent(JsonConvert.SerializeObject(u));
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                HttpResponseMessage response = await httpClient.PostAsync(uriBase + uriComplementar, content);

                string serialized = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    UsuarioViewModel uLogado = JsonConvert.DeserializeObject<UsuarioViewModel>(serialized);
                    HttpContext.Session.SetString("SessionTokenUsuario", uLogado.Token);
                    HttpContext.Session.SetString("SessionUsername", uLogado.Username);
                    TempData["Mensagem"] = string.Format("Bem-Vindo {0}!!", uLogado.Username);
                    return RedirectToAction("index", "Personagens");
                }
                else
                {
                    throw new System.Exception(serialized);
                }
            }
            catch (System.Exception ex)
            {
                TempData["MensagemErro"] = ex.Message;
                return IndexLogin();
            }
        }

        [HttpGet]
        public async Task<ActionResult> IndexInformacoesAsync()
        {
            try
            {
                HttpClient httpClient = new HttpClient();
                //Novo: Recuperação informação da sessão
                string login = HttpContext.Session.GetString("SessionUsername");
                string uriComplementar =
                $"GetByLogin/{login}";
                HttpResponseMessage response = await httpClient.GetAsync(uriBase +
                uriComplementar);
                string serialized = await response.Content.ReadAsStringAsync();
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    UsuarioViewModel u = await Task.Run(() =>
                    JsonConvert.DeserializeObject<UsuarioViewModel>(serialized));
                    return View(u);
                }
                else
                {
                    throw new System.Exception(serialized);
                }
            }
            catch (System.Exception ex)
            {
                TempData["MensagemErro"] = ex.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<ActionResult> AlterarEmail(UsuarioViewModel u)
        {
            try
            {
                HttpClient httpClient = new HttpClient();
                string token = HttpContext.Session.GetString("SessionTokenUsuario");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                string uriComplementar = "AtualizarEmail";
                var content = new StringContent(JsonConverte.SerializeObject(u));
                content.Headers.ContentType = new MediaTypeHeaderValue("application;json");

                HttpResponseMessage response = await httpClient.PutAsync(uriBase + uriComplementar, content);
                string serialized = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    TempData["Mensagem"] = "E-mail alterado com Sucesso";
                else   
                    throw new System.Exception(serialized);
            }
            catch (System.Exception ex)
            {
                TempData["MensagemErro"] = ex.Message;
            }
            return RedirectToAction("IndexInformacoes");
        }
    }
}
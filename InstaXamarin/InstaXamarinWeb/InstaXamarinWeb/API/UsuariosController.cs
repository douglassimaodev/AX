﻿using InstaXamarinWeb.Models;
using InstaXamarinWeb.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace InstaXamarinWeb.API
{
    public class UsuariosController : ApiController
    {
        private DB db = new DB();

        //Login
        [Route("api/usuario/login")]
        [HttpPost]
        public HttpResponseMessage Login(dynamic DadosLogin)
        {
            String Email = DadosLogin.Email.ToString();
            String Senha = Utilitarios.HashPassword(DadosLogin.Senha.ToString());

            Usuario usuario = db.Usuarios.FirstOrDefault(uu => uu.Email == Email && uu.Senha == Senha && uu.Bloqueado == false);

            if (usuario != null)
            {

                AccessToken AT = new AccessToken();
                AT.UsuarioId = usuario.Id;

                AT.Token = Guid.NewGuid().ToString();

                AT.Data = DateTime.Now;

                db.AccessTokens.Add(AT);
                db.SaveChanges();

                HttpResponseMessage Response = Request.CreateResponse(HttpStatusCode.OK, usuario);
                Response.Headers.Add("token", AT.Token);
                return Response;
            }
            else
                return Request.CreateResponse(HttpStatusCode.NotFound);
        }

        //Logoff
        [Route("api/usuario/logoff")]
        [HttpPost]
        public HttpResponseMessage LogOff()
        {
            AccessToken AT = Util.Utilitarios.GetToken(Request);
            if (AT != null)
            {
                db.AccessTokens.Remove(AT);
                db.SaveChanges();

                HttpResponseMessage Response = Request.CreateResponse(HttpStatusCode.OK);
                Response.Headers.Add("token", "");
                return Response;
            }
            else
                return Request.CreateResponse(HttpStatusCode.NotFound);
        }

        //Cadastra Usuario
        [Route("api/usuario")]
        [HttpPost]
        public HttpResponseMessage Cadastro(dynamic user)
        {
            String Email = user.Email.ToString();
            Usuario usuario = db.Usuarios.FirstOrDefault(uu => uu.Email == Email);
            if (usuario != null)
                return Request.CreateResponse(HttpStatusCode.NotAcceptable, "Email já está sendo usado");

            usuario = new Usuario();

            usuario.Nome = user.Nome.ToString();
            usuario.Email = user.Email.ToString();
            usuario.Senha = Utilitarios.HashPassword(user.Senha.ToString());

            usuario.DataCadastro = DateTime.Now;

            db.Usuarios.Add(usuario);
            db.SaveChanges();


            AccessToken AT = new AccessToken();
            AT.UsuarioId = usuario.Id;
            AT.Data = DateTime.Now;

            AT.Token = Guid.NewGuid().ToString();

            db.AccessTokens.Add(AT);
            db.SaveChanges();

            HttpResponseMessage Response = Request.CreateResponse(HttpStatusCode.OK, usuario);
            Response.Headers.Add("token", AT.Token);
            return Response;
        }


        //Altera dados do Usuário
        [APIAutorizacao]
        [Route("api/usuario")]
        [HttpPut]
        public IHttpActionResult AlteraDados([FromBody]dynamic user)
        {
            Usuario USUARIO_EXISTENTE = db.Usuarios.Find(Util.Utilitarios.GetTokenUsuarioLogado(Request));

            USUARIO_EXISTENTE.Nome = user.Nome.ToString();
            USUARIO_EXISTENTE.Email = user.Email.ToString();
            if (!String.IsNullOrEmpty(user.Senha.ToString()))
                USUARIO_EXISTENTE.Senha = Utilitarios.HashPassword(user.Senha.ToString());

            db.SaveChanges();

            return Ok(USUARIO_EXISTENTE);
        }

        //Altera Foto
        [Route("api/usuario/foto")]
        [HttpPut]
        public IHttpActionResult AlteraFoto([FromBody]String FotoBase64)
        {
            Usuario USUARIO = db.Usuarios.Find(Util.Utilitarios.GetTokenUsuarioLogado(Request));
            try
            {
                USUARIO.Foto = FotoBase64.ToByteArray().Redimenciona(200, 200);
                db.SaveChanges();

                return StatusCode(HttpStatusCode.NoContent);
            }
            catch (Exception EX)
            {
                return InternalServerError(EX);
            }
        }


        //Get Foto
        [Route("api/usuario/{UsuarioID}/foto")]
        [HttpGet]
        public HttpResponseMessage Foto(int UsuarioID)
        {
            Usuario usuario = db.Usuarios.Find(UsuarioID);

            if (usuario != null && usuario.Foto != null)
            {
                HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
                result.Content = new StreamContent(new MemoryStream(usuario.Foto));
                result.Content.Headers.ContentType = new MediaTypeHeaderValue(System.Net.Mime.MediaTypeNames.Image.Jpeg);
                return result;
            }

            return new HttpResponseMessage(HttpStatusCode.NotAcceptable);
        }


        [Route("api/usuario/add_token_push")]
        [HttpPut]
        public HttpResponseMessage Login([FromBody]UsuarioToken UT)
        {
            Usuario usuario = db.Usuarios.Find(Util.Utilitarios.GetTokenUsuarioLogado(Request));

            if (usuario != null)
            {
                UT.UsuarioId = usuario.Id;

                UT.Data = DateTime.Now;

                db.UsuarioTokens.Add(UT);
                db.SaveChanges();

                HttpResponseMessage Response = Request.CreateResponse(HttpStatusCode.OK, UT);
                return Response;
            }
            else
                return Request.CreateResponse(HttpStatusCode.NotFound);
        }


        //Cancela Conta
        [Route("api/usuario")]
        [HttpDelete]
        public IHttpActionResult ApagaUsuario()
        {
            Usuario usuario = db.Usuarios.Find(Util.Utilitarios.GetTokenUsuarioLogado(Request));
            if (usuario == null)
                return NotFound();

            db.Usuarios.Remove(usuario);
            db.SaveChanges();

            return StatusCode(HttpStatusCode.NoContent);
        }


        //Seguir
        [Route("api/usuario/seguir/{SeguidoID}")]
        [HttpPut]
        public IHttpActionResult Seguir(int SeguidoID)
        {
            Seguidor SEGUIDOR = db.Seguidores.Find(Util.Utilitarios.GetTokenUsuarioLogado(Request), SeguidoID);

            if (SEGUIDOR == null)
            {
                SEGUIDOR = new Seguidor();
                SEGUIDOR.SeguidorID = Util.Utilitarios.GetTokenUsuarioLogado(Request);
                SEGUIDOR.SeguidoID = SeguidoID;

                db.Seguidores.Add(SEGUIDOR);

                Usuario Seguidor = db.Usuarios.Find(Util.Utilitarios.GetTokenUsuarioLogado(Request));
                Seguidor.QuantidadeSeguindo++;
                Usuario Seguido = db.Usuarios.Find(SeguidoID);
                Seguidor.QuantidadeSeguidores++;

                db.SaveChanges();
            }

            return Ok();
        }

        //Deixar de Seguir
        [Route("api/usuario/deixar-seguir/{SeguidoID}")]
        [HttpPut]
        public IHttpActionResult DeixarSeguir(int SeguidoID)
        {
            Seguidor SEGUIDOR = db.Seguidores.Find(Util.Utilitarios.GetTokenUsuarioLogado(Request), SeguidoID);

            if (SEGUIDOR != null)
            {
                Usuario Seguidor = db.Usuarios.Find(SEGUIDOR.SeguidorID);
                Seguidor.QuantidadeSeguindo--;
                Usuario Seguido = db.Usuarios.Find(SEGUIDOR.SeguidoID);
                Seguidor.QuantidadeSeguidores--;

                db.Seguidores.Remove(SEGUIDOR);
                db.SaveChanges();
            }

            return Ok();
        }

        //Lista Seguidores
        [Route("api/usuario/{UsuarioID}/seguidores")]
        [HttpGet]
        public IHttpActionResult ListaSeguidores(int UsuarioID)
        {
            List<int> SeguidoresIDs = db.Seguidores.Where(ss => ss.SeguidoID == UsuarioID).Select(ss => ss.SeguidorID).ToList();

            List<Usuario> Seguidores = db.Usuarios.Where(uu => SeguidoresIDs.Contains(uu.Id)).ToList();

            return Ok(Seguidores);
        }

        //Lista Seguidos
        [Route("api/usuario/{UsuarioID}/seguidos")]
        [HttpGet]
        public IHttpActionResult ListaSeguidos(int UsuarioID)
        {
            List<int> SeguidosIDs = db.Seguidores.Where(ss => ss.SeguidorID == UsuarioID).Select(ss => ss.SeguidoID).ToList();

            List<Usuario> Seguidos = db.Usuarios.Where(uu => SeguidosIDs.Contains(uu.Id)).ToList();

            return Ok(Seguidos);
        }



        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool UsuarioExists(int id)
        {
            return db.Usuarios.Count(e => e.Id == id) > 0;
        }

    }
}

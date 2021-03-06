﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstaXamarinMobile.Models
{
    public class PostInteracao
    {
        public int Id { get; set; }

        public int PostId { get; set; }

        public int ComentarioId { get; set; }

        public int UsuarioId { get; set; }

        public Usuario UsuarioDados { get; set; }


        public enum TipoInteracao
        {
            Curtida = 1,
            Comentario = 2,
            DenunciaPost = 3,
            DenunciaComentario = 4
        }
        public TipoInteracao Tipo { get; set; }

        public string Texto { get; set; }
        public DateTime Data { get; set; }

        //Para Comentários
        public int QuantidadeDenuncias { get; set; }
        public bool Denunciado { get; set; }
        public bool Bloqueado { get; set; }
    }
}

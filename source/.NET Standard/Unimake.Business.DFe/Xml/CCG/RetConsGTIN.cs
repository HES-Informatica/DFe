﻿#if INTEROP
using System.Runtime.InteropServices;
#endif
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Unimake.Business.DFe.Servicos;
using Unimake.Business.DFe.Utility;
using System.Globalization;

namespace Unimake.Business.DFe.Xml.CCG
{
    /// <summary>
    /// Classe para serialização e deserialização do XML de consulta centralizada do código GTIN
    /// </summary>
#if INTEROP
    [ClassInterface(ClassInterfaceType.AutoDual)]
    [ProgId("Unimake.Business.DFe.Xml.CCG.RetConsGTIN")]
    [ComVisible(true)]
#endif
    [XmlRoot("retConsGTIN", Namespace = "http://www.portalfiscal.inf.br/nfe", IsNullable = false)]
    public class RetConsGTIN : XMLBase
    {
        /// <summary>
        /// Versão do schema XML
        /// </summary>
        [XmlAttribute(AttributeName = "versao", DataType = "token")]
        public string Versao { get; set; }

        /// <summary>
        /// Versão da aplicação que atendeu a requisição
        /// </summary>
        [XmlElement("verAplic")]
        public string VerAplic { get; set; }

        /// <summary>
        /// Código do status da resposta. Se não tiver erro, será retornado: “9490 – Consulta realizada com sucesso“
        /// </summary>
        [XmlElement("cStat")]
        public int CStat { get; set; }

        /// <summary>
        /// Descrição do status da resposta
        /// </summary>
        [XmlElement("xMotivo")]
        public string XMotivo { get; set; }

        /// <summary>
        /// Data e hora da resposta
        /// </summary>
        [XmlIgnore]
        public DateTimeOffset DhResp { get; set; }

        /// <summary>
        /// Data e hora da resposta (Obs: Utilize a propriedade DhResp para atribuir o valor)
        /// </summary>
        [XmlElement("dhResp")]
        public string DhRespField
        {
            get => DhResp.ToString("yyyy-MM-ddTHH:mm:sszzz", CultureInfo.InvariantCulture);
            set => DhResp = DateTimeOffset.Parse(value);
        }

        private string GTINField;

        /// <summary>
        /// Código GTIN consultado
        /// </summary>
        [XmlElement("GTIN")]
        public string GTIN
        {
            get => GTINField;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new Exception("Código GTIN inválido! Não pode ser nulo ou espaço em branco.");
                }

                if (value.Length != 8 && value.Length != 12 && value.Length != 13 && value.Length != 14)
                {
                    throw new Exception("Código GTIN informado (" + value + ") inválido. GTIN deve ter, como tamanho, 8,12,13 ou 14 números sem conter letras.");
                }

                for (var i = 0; i < value.Length; i++)
                {
                    if (!"0123456789".Contains(value.Substring(i, 1)))
                    {
                        throw new Exception("Código GTIN informado (" + value + ") inválido. Não pode conter letras, somente números.");
                    }
                }

                GTINField = value;
            }
        }

        /// <summary>
        /// Tipos possíveis: 8, 12, 13, 14
        /// </summary>
        [XmlElement("tpGTIN")]
        public TipoCodigoGTIN TpGTIN { get; set; }

        /// <summary>
        /// Descrição do Produto, cadastrada pelo “Dono da Marca” na GS1, para o GTIN consultado
        /// </summary>
        [XmlElement("xProd")]
        public string XProd { get; set; }

        /// <summary>
        /// Código do NCM, cadastrado pelo “Dono da Marca” na GS1, para o GTIN consultado
        /// </summary>
        [XmlElement("NCM")]
        public string NCM { get; set; }

        /// <summary>
        /// Código do CEST, cadastrado pelo “Dono da Marca” na´GS1. Normalmente um Produto (definido pelo código do GTIN) está vinculado a somente 1 CEST, mas existem situações pouco frequentes onde um Produto pode estar associado a mais de 1 CEST, conforme a operação.
        /// </summary>
        [XmlElement("CEST")]
        public List<string> CEST { get; set; }

        /// <summary>
        /// Deserializar o XML retConsGTIN no objeto RetConsGTIN.
        /// </summary>
        /// <param name="filename">Localização do arquivo XML retConsGTIN</param>
        /// <returns>Objeto do RetConsGTIN</returns>
        public RetConsGTIN LoadFromFile(string filename)
        {
            var doc = new XmlDocument();
            doc.LoadXml(System.IO.File.ReadAllText(filename, Encoding.UTF8));
            return XMLUtility.Deserializar<RetConsGTIN>(doc);
        }

        /// <summary>
        /// Deserializar o XML retConsGTIN no objeto RetConsGTIN.
        /// </summary>
        /// <param name="xml">string do XML retConsGTIN</param>
        /// <returns>Objeto do RetConsGTIN</returns>
        public RetConsGTIN LoadFromXML(string xml) => XMLUtility.Deserializar<RetConsGTIN>(xml);
    }
}
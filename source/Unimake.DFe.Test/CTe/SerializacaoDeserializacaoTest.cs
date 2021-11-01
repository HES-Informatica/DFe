﻿using System.Diagnostics;
using System.IO;
using System.Xml;
using Unimake.Business.DFe.Servicos;
using Unimake.Business.DFe.Xml.CTe;
using Unimake.Business.DFe.Xml.CTeOS;
using Xunit;

namespace Unimake.DFe.Test.CTe
{
    /// <summary>
    /// Testar a serialização e deserialização dos XMLs do CTe
    /// </summary>
    public class SerializacaoDeserializacaoTest
    {
        /// <summary>
        /// Testar a serialização e deserialização do XML EnviCTe
        /// </summary>
        [Theory]
        [Trait("DFe", "CTe")]
        [InlineData(@"..\..\..\CTe\Resources\enviCTe_ModalAereo.xml")]
        [InlineData(@"..\..\..\CTe\Resources\enviCTe_ModalAquaviario.xml")]
        [InlineData(@"..\..\..\CTe\Resources\enviCTe_ModalDutoviario.xml")]
        [InlineData(@"..\..\..\CTe\Resources\enviCTe_ModalFerroviario.xml")]
        [InlineData(@"..\..\..\CTe\Resources\enviCTe_ModalMultiModal.xml")]
        [InlineData(@"..\..\..\CTe\Resources\enviCTe_ModalRodoviario.xml")]
        public void SerializacaoDeserializacaoEnviMDFe(string arqXML)
        {
            Debug.Assert(File.Exists(arqXML), "Arquivo " + arqXML + " não foi localizado para a realização da serialização/deserialização.");

            var doc = new XmlDocument();
            doc.Load(arqXML);

            var xml = new EnviCTe();
            xml = xml.LerXML<EnviCTe>(doc);

            var configuracao = new Configuracao
            {
                TipoDFe = TipoDFe.CTe,
                CertificadoDigital = PropConfig.CertificadoDigital
            };

            var autorizacao = new Unimake.Business.DFe.Servicos.CTe.Autorizacao(xml, configuracao);

            Debug.Assert(doc.InnerText == autorizacao.ConteudoXMLOriginal.InnerText, "XML gerado pela DLL está diferente do conteúdo do arquivo serializado.");
        }

        /// <summary>
        /// Testar a serialização e deserialização do XML CTeOS
        /// </summary>
        [Theory]
        [Trait("DFe", "CTe")]
        [InlineData(@"..\..\..\CTe\Resources\CTeOS_ModalRodoOS.xml")]
        public void SerializacaoDeserializacaoCTeOS(string arqXML)
        {
            Debug.Assert(File.Exists(arqXML), "Arquivo " + arqXML + " não foi localizado para a realização da serialização/deserialização.");

            var doc = new XmlDocument();
            doc.Load(arqXML);

            var xml = new CTeOS();
            xml = xml.LerXML<CTeOS>(doc);

            var configuracao = new Configuracao
            {
                TipoDFe = TipoDFe.CTeOS,
                CertificadoDigital = PropConfig.CertificadoDigital
            };

            var autorizacao = new Unimake.Business.DFe.Servicos.CTeOS.Autorizacao(xml, configuracao);

            Debug.Assert(doc.InnerText == autorizacao.ConteudoXMLOriginal.InnerText, "XML gerado pela DLL está diferente do conteúdo do arquivo serializado.");
        }

    }
}
﻿using System;
using System.Collections.Generic;
using Diag = System.Diagnostics;
using System.IO;
using System.Xml;
using Unimake.Business.DFe.Servicos;
using Unimake.Business.DFe.Servicos.NFSe;
using Xunit;

namespace Unimake.DFe.Test.NFSe
{
    /// <summary>
    /// Testar o serviço: CancelarNfse
    /// </summary>
    public class CancelarNfseTest
    {
        /// <summary>
        /// Monta o parâmetros, de forma dinâmica, para o cenário de testes
        /// </summary>
        public static IEnumerable<object[]> Parametros => TestUtility.PreparaDadosCenario("CancelarNfse");

        /// <summary>
        /// Cancelar NFse para saber se a conexão com o webservice está ocorrendo corretamente.
        /// </summary>
        /// <param name="tipoAmbiente">Ambiente para onde deve ser enviado o XML</param>
        [Theory]
        [Trait("DFe", "NFSe")]
        [MemberData(nameof(Parametros))]
        public void CancelarNfse(TipoAmbiente tipoAmbiente, PadraoNFSe padraoNFSe, string versaoSchema, int codMunicipio, string nomeMunicipio)
        {
            var nomeXMLEnvio = "CancelarNfseEnvio-ped-cannfse.xml";
            
            string arqXML;

            switch (padraoNFSe)
            {
                case PadraoNFSe.NOBESISTEMAS:
                    arqXML = "..\\..\\..\\NFSe\\Resources\\" + padraoNFSe.ToString() + "\\" + versaoSchema + "\\" + tipoAmbiente.ToString() + "\\" + nomeXMLEnvio;
                    break;

                default:
                    arqXML = "..\\..\\..\\NFSe\\Resources\\" + padraoNFSe.ToString() + "\\" + versaoSchema + "\\" + nomeXMLEnvio;
                    break;
            }

            Diag.Debug.Assert(File.Exists(arqXML), "Arquivo " + arqXML + " não foi localizado.");

            try
            {
                var conteudoXML = new XmlDocument();
                conteudoXML.Load(arqXML);

                var configuracao = new Configuracao
                {
                    TipoDFe = TipoDFe.NFSe,
                    CertificadoDigital = PropConfig.CertificadoDigital,
                    TipoAmbiente = tipoAmbiente,
                    CodigoMunicipio = codMunicipio,
                    Servico = Servico.NFSeCancelarNfse,
                    SchemaVersao = versaoSchema,
                    MunicipioToken = "99n0556af8e4218e05b88e266fhca55be17b14a4495c269d1db0af57f925f04e77c38f9870842g5g60b6827a9fje8ec9" //Tem município que exige token, então já vamos deixar algo definido para que utilize nos padrões necessários durante o teste unitário. Não é obrigatório para todos os padrões e será utilizado somente nos que solicitam.
                };

                var cancelarNfse = new CancelarNfse(conteudoXML, configuracao);
                cancelarNfse.Executar();
            }
            catch(Exception ex)
            {
                Diag.Debug.Assert(false, "Falha na hora de consumir o serviço: " + nomeMunicipio + " - IBGE: " + codMunicipio + " - Padrão: " + padraoNFSe.ToString() + " - Versão schema: " + versaoSchema + "\r\nExceção: " + ex.Message, ex.StackTrace);
            }
        }
    }
}
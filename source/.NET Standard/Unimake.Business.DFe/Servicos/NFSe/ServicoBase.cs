﻿#if INTEROP
using System.Runtime.InteropServices;
#endif
using System;
using System.IO;
using System.Linq.Expressions;
using System.Text;
using System.Xml;
using Unimake.Business.DFe.Security;
using Unimake.Exceptions;

namespace Unimake.Business.DFe.Servicos.NFSe
{
    /// <summary>
    /// Classe base para os serviços da NFSe
    /// </summary>
#if INTEROP
    [ClassInterface(ClassInterfaceType.AutoDual)]
    [ProgId("Unimake.Business.DFe.Servicos.NFSe.ServicoBase")]
    [ComVisible(true)]
#endif
    public abstract class ServicoBase : Servicos.ServicoBase
    {
        /// <summary>
        /// Construtor
        /// </summary>
        protected ServicoBase() : base() { }


        /// <summary>
        /// Definir configurações
        /// </summary>
        protected override void DefinirConfiguracao() { }

        /// <summary>
        /// Ajustes no XMLs, depois de assinado.
        /// </summary>
        protected override void AjustarXMLAposAssinado()
        {
            #region Resolver problema da assinatura de Uberlândia-MG, que fugiu padrão mundial

            if (Configuracoes.CodigoMunicipio == 3170206) //Uberlândia (Tem esta zica na assinatura, pensa em merda.)
            {
                if (Configuracoes.Servico == Servico.NFSeRecepcionarLoteRps ||
                    Configuracoes.Servico == Servico.NFSeRecepcionarLoteRpsSincrono ||
                    Configuracoes.Servico == Servico.NFSeGerarNfse ||
                    Configuracoes.Servico == Servico.NFSeSubstituirNfse || 
                    Configuracoes.Servico == Servico.NFSeCancelarNfse)
                {

                    var xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(ConteudoXML.OuterXml);
                    var mudouXml = false;
                    if (Configuracoes.TagAssinatura.Equals("Rps"))
                    {
                        if (Configuracoes.Servico == Servico.NFSeGerarNfse)
                        {
                            var nodeRps = xmlDoc.GetElementsByTagName("Rps")[0];
                            var elementNodeRps = (XmlElement)nodeRps;
                            var elementInfDeclaracao = (XmlElement)elementNodeRps.GetElementsByTagName("InfDeclaracaoPrestacaoServico")[0];
                            var id = elementInfDeclaracao.GetAttribute("Id").Replace("ID_", "");
                            var elementSignatureValue = (XmlElement)elementNodeRps.GetElementsByTagName("SignatureValue")[0];

                            if (string.IsNullOrWhiteSpace(elementSignatureValue.GetAttribute("Id")))
                            {
                                var attributeId = xmlDoc.CreateAttribute("Id");
                                attributeId.Value = "ID_ASSINATURA_" + id;
                                elementSignatureValue.SetAttributeNode(attributeId);

                                mudouXml = true;
                            }
                        }
                        else
                        {
                            var listListaRps = xmlDoc.GetElementsByTagName("ListaRps");
                            foreach (XmlNode nodeListaRps in listListaRps)
                            {
                                var elementListaRps = (XmlElement)nodeListaRps;
                                foreach (XmlNode nodeRps in elementListaRps.GetElementsByTagName("Rps"))
                                {
                                    var elementRps = (XmlElement)nodeRps;
                                    if (elementRps.GetElementsByTagName("InfDeclaracaoPrestacaoServico").Count > 0)
                                    {
                                        var elementInfDeclaracao = (XmlElement)elementRps.GetElementsByTagName("InfDeclaracaoPrestacaoServico")[0];
                                        var id = elementInfDeclaracao.GetAttribute("Id").Replace("ID_", "");
                                        var elementSignatureValue = (XmlElement)elementRps.GetElementsByTagName("SignatureValue")[0];

                                        if (string.IsNullOrWhiteSpace(elementSignatureValue.GetAttribute("Id")))
                                        {
                                            var attributeId = xmlDoc.CreateAttribute("Id");
                                            attributeId.Value = "ID_ASSINATURA_" + id;
                                            elementSignatureValue.SetAttributeNode(attributeId);
                                            mudouXml = true;
                                        }
                                    }
                                }
                            }
                        }

                        if (mudouXml)
                        {
                            ConteudoXML.LoadXml(xmlDoc.OuterXml);
                        }
                    }
                    else if (Configuracoes.TagAssinatura.Equals("Pedido")) //Para o serviço CancelarNfse
                    {
                        var nodePedido = xmlDoc.GetElementsByTagName("Pedido")[0];
                        var elementNodePedido = (XmlElement)nodePedido;
                        var elementInfPedidoCancelamento = (XmlElement)elementNodePedido.GetElementsByTagName("InfPedidoCancelamento")[0];
                        var id = elementInfPedidoCancelamento.GetAttribute("Id").Replace("ID_PEDIDO_CANCELAMENTO_", "");
                        var elementSignatureValue = (XmlElement)elementNodePedido.GetElementsByTagName("SignatureValue")[0];

                        if (string.IsNullOrWhiteSpace(elementSignatureValue.GetAttribute("Id")))
                        {
                            var attributeId = xmlDoc.CreateAttribute("Id");
                            attributeId.Value = "ID_ASSINATURA_PEDIDO_CANCELAMENTO_" + id;
                            elementSignatureValue.SetAttributeNode(attributeId);

                            mudouXml = true;
                        }
                    }

                    if (mudouXml)
                    {
                        ConteudoXML.LoadXml(xmlDoc.OuterXml);
                    }

                }
            }

            #endregion
        }

        /// <summary>
        /// Conteúdo do XML assinado.
        /// </summary>
        public override XmlDocument ConteudoXMLAssinado
        {
            get
            {
                if (Configuracoes.PadraoNFSe == PadraoNFSe.DSF && Configuracoes.EncriptaTagAssinatura)
                {
                    var sh1 = Criptografia.GetSHA1HashData(ConteudoXML.GetElementsByTagName("Assinatura")[0].InnerText);
                    ConteudoXML.GetElementsByTagName("Assinatura")[0].InnerText = sh1;
                }

                ConteudoXML = base.ConteudoXMLAssinado;

                return ConteudoXML;
            }
        }

        /// <summary>
        /// Validar o XML
        /// </summary>
        protected override void XmlValidar()
        {
            XmlValidarConteudo(); // Efetuar a validação antes de validar schema para evitar alguns erros que não ficam claros para o desenvolvedor.

            if (!string.IsNullOrWhiteSpace(Configuracoes.SchemaArquivo))
            {
                var validar = new ValidarSchema();
                validar.Validar(ConteudoXML,
                    Configuracoes.TipoDFe.ToString() + "." + Configuracoes.PadraoNFSe.ToString() + "." + Configuracoes.SchemaArquivo,
                    Configuracoes.TargetNS,
                    Configuracoes.PadraoNFSe);

                if (!validar.Success)
                {
                    throw new ValidarXMLException(validar.ErrorMessage);
                }
            }
        }

        /// <summary>
        /// Inicializa configurações, parâmetros e propriedades para execução do serviço.
        /// </summary>
        /// <param name="conteudoXML">Conteúdo do XML a ser enviado para o web-service</param>
        /// <param name="configuracao">Configurações a serem utilizadas para conexão e envio do XML para o web-service</param>
#if INTEROP
        [ComVisible(false)]
#endif
        protected override void Inicializar(XmlDocument conteudoXML, Configuracao configuracao)
        {
            if (configuracao.PadraoNFSe == PadraoNFSe.ABASE || configuracao.PadraoNFSe == PadraoNFSe.BETHA || configuracao.PadraoNFSe == PadraoNFSe.GINFES ||
                configuracao.PadraoNFSe == PadraoNFSe.MEMORY || configuracao.PadraoNFSe == PadraoNFSe.EQUIPLANO || configuracao.PadraoNFSe == PadraoNFSe.WEBFISCO)
            {
                //Municípios pontuais com configuração diferente:
                //São José dos Pinhais - PR     |GINFES
                //Varginha - MG                 |BETHA
                if (configuracao.CodigoMunicipio != 4125506 || configuracao.CodigoMunicipio != 3170701)
                {
                    configuracao.CodigoMunicipio = (int)(CodigoPadraoNFSe)Enum.Parse(typeof(CodigoPadraoNFSe), configuracao.PadraoNFSe.ToString());
                }
            }

            base.Inicializar(conteudoXML, configuracao);
        }

        /// <summary>
        /// Validar, o conteúdo das tags do XML, alguns validações manuais que o schema não faz. Vamos implementando novas regras na medida da necessidade de cada serviço.
        /// </summary>
        protected override void XmlValidarConteudo() { }

#if INTEROP

        /// <summary>
        /// Executa o serviço: Assina o XML, valida e envia para o web-service
        /// </summary>
        /// <param name="conteudoXML">Conteúdo do XML que será enviado para o WebService</param>
        /// <param name="configuracao">Objeto "Configuracoes" com as propriedade necessária para a execução do serviço</param>
        [ComVisible(true)]
        public void Executar(string conteudoXML, Configuracao configuracao)
        {
            try
            {
                if (configuracao is null)
                {
                    throw new ArgumentNullException(nameof(configuracao));
                }

                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(conteudoXML);

                Inicializar(xmlDoc, configuracao);

                Executar();
            }
            catch (ValidarXMLException ex)
            {
                ThrowHelper.Instance.Throw(ex);
            }
            catch (CertificadoDigitalException ex)
            {
                ThrowHelper.Instance.Throw(ex);
            }
            catch (Exception ex)
            {
                ThrowHelper.Instance.Throw(ex);
            }
        }

#endif

        /// <summary>
        /// Executa o serviço: Assina o XML, valida e envia para o webservice
        /// </summary>
#if INTEROP
        [ComVisible(false)]
#endif
        public override void Executar()
        {

            if (Configuracoes.UsaCertificadoDigital && Configuracoes.NaoAssina == null && Configuracoes.NaoAssina != Configuracoes.TipoAmbiente)
            {
                if (!string.IsNullOrWhiteSpace(Configuracoes.TagAssinatura) && !AssinaturaDigital.EstaAssinado(ConteudoXML, Configuracoes.TagAssinatura))
                {
                    AssinaturaDigital.Assinar(ConteudoXML, Configuracoes.TagAssinatura, Configuracoes.TagAtributoID, Configuracoes.CertificadoDigital, AlgorithmType.Sha1, true, "Id");
                }

                if (!string.IsNullOrWhiteSpace(Configuracoes.TagLoteAssinatura) && !AssinaturaDigital.EstaAssinado(ConteudoXML, Configuracoes.TagLoteAssinatura))
                {
                    AssinaturaDigital.Assinar(ConteudoXML, Configuracoes.TagLoteAssinatura, Configuracoes.TagLoteAtributoID, Configuracoes.CertificadoDigital, AlgorithmType.Sha1, true, "Id");
                }
            }

            AjustarXMLAposAssinado();

            XmlValidar();

            base.Executar();
        }


        /// <summary>
        /// Gravar o XML de distribuição em uma pasta no HD
        /// </summary>
        /// <param name="pasta">Pasta onde deve ser gravado o XML no HD</param>
        /// <param name="nomeArquivo">Nome do arquivo a ser gravado no HD</param>
        /// <param name="conteudoXML">String contendo o conteúdo do XML a ser gravado no HD</param>
#if INTEROP
        [ComVisible(false)]
#endif
        public override void GravarXmlDistribuicao(string pasta, string nomeArquivo, string conteudoXML)
        {
            StreamWriter streamWriter = null;

            try
            {
                var conteudoXmlDistribuicao = conteudoXML;

                streamWriter = File.CreateText(Path.Combine(pasta, nomeArquivo));
                streamWriter.Write(conteudoXmlDistribuicao);
            }
            finally
            {
                if (streamWriter != null)
                {
                    streamWriter.Close();
                }
            }
        }

        /// <summary>
        /// Gravar o XML de distribuição em um stream
        /// </summary>
        /// <param name="value">Conteúdo a ser gravado no stream</param>
        /// <param name="stream">Stream que vai receber o conteúdo do XML</param>
        /// <param name="encoding">Define o encodingo do stream, caso não informado ,será usado o UTF8</param>
#if INTEROP
        [ComVisible(false)]
#endif
        public virtual void GravarXmlDistribuicao(Stream stream,
                                                  string value,
                                                  Encoding encoding = null)
        {
            if (stream is null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (encoding == null)
            {
                encoding = Encoding.UTF8;
            }

            var byteData = encoding.GetBytes(value);
            stream.Write(byteData, 0, byteData.Length);
            stream.Close();
        }
    }
}
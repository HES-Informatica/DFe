* ---------------------------------------------------------------------------------
* Enviar evento de encerramento do MDFe
* ---------------------------------------------------------------------------------
FUNCTION EnviarEventoEncerramentoMDFe()
   LOCAL oErro, oExceptionInterop
   LOCAL oConfiguracao
   LOCAL oEnvEvento, oEventoMDFe, oDetEventoCanc, oInfEvento
   
 * Criar configuração basica para consumir o serviço
   oConfiguracao = CREATEOBJECT("Unimake.Business.DFe.Servicos.Configuracao")
   oConfiguracao.TipoDfe = 4 && 4=MDFe
   oConfiguracao.CertificadoSenha = "12345678"
   oConfiguracao.CertificadoArquivo = "C:\Projetos\certificados\UnimakePV.pfx"

 * Criar tag Evento
   oEventoMDFe = CREATEOBJECT("Unimake.Business.DFe.Xml.MDFe.EventoMDFe")
   oEventoMDFe.Versao = "3.00"
 
 * Criar tag DetEventoEncMDFe
   oDetEventoEncMDFe = CREATEOBJECT("Unimake.Business.DFe.Xml.MDFe.DetEventoEncMDFe") && ###
   oDetEventoEncMDFe.VersaoEvento = "3.00"
   oDetEventoEncMDFe.NProt = "141200000007987"
   oDetEventoEncMDFe.CMun = 3106200
   oDetEventoEncMDFe.CUF = 41
   oDetEventoEncMDFe.DtEnc = DateTime()   

 * Criar tag InfEvento
   oInfEvento = CREATEOBJECT("Unimake.Business.DFe.Xml.MDFe.InfEvento")
 
 * Adicionar a tag DetEventoCanc dentro da Tag DetEvento
   oInfEvento.DetEvento = oDetEventoEncMDFe
 
 * Atualizar propriedades da oInfEvento
 * IMPORTANTE: Atualização da propriedade TpEvento deve acontecer depois que o DetEvento recebeu o oDetEventoCanc para que funcione sem erro
   oInfEvento.COrgao = 41 && UFBrasil.PR
   oInfEvento.ChMDFe = "41200210859283000185570010000005671227070615"
   oInfEvento.CNPJ = "10859283000185"
   oInfEvento.DhEvento = DateTime()
   oInfEvento.TpEvento = 110112 && TipoEventoNFe.Encerramento ###
   oInfEvento.NSeqEvento = 1
   oInfEvento.TpAmb = 2 && TipoAmbiente.Homologacao

 * Adicionar a tag InfEvento dentro da tag Evento
   oEventoMDFe.InfEvento = oInfEvento

 * Resgatando alguns dados do objeto do XML do evento
   MESSAGEBOX("<versao>: " + oEventoMDFe.Versao)
   MESSAGEBOX("<cOrgao>: " + STR(oEventoMDFe.InfEvento.COrgao,2))
   MESSAGEBOX("<chMDFe>: " + oEventoMDFe.InfEvento.ChMDFe)
   MESSAGEBOX("<nProt>: " + oEventoMDFe.InfEvento.DetEvento.NProt)
   
 * Criar objeto para pegar exceção do lado do CSHARP
   oExceptionInterop = CREATEOBJECT("Unimake.Exceptions.ThrowHelper")   
   
   TRY 
    * Enviar evento
      oRecepcaoEvento = CREATEOBJECT("Unimake.Business.DFe.Servicos.MDFe.RecepcaoEvento")
      oRecepcaoEvento.Executar(oEventoMDFe,  oConfiguracao)
      
      eventoAssinado = oRecepcaoEvento.GetConteudoXMLAssinado()
      MESSAGEBOX(eventoAssinado)
      
    * Gravar o XML assinado no HD.
      DELETE FILE 'd:\testenfe\EncerramentoMDFe.xml'
	  STRTOFILE(eventoAssinado, 'd:\testenfe\EncerramentoMDFe.xml', 0)
      
      MESSAGEBOX(oRecepcaoEvento.RetornoWSString)
	  
      MESSAGEBOX("CStat do Lote Retornado: " + ALLTRIM(STR(oRecepcaoEvento.Result.InfEvento.CStat,5)) + " - XMotivo: " + oRecepcaoEvento.Result.InfEvento.XMotivo)
 
      IF oRecepcaoEvento.Result.InfEvento.CStat == 135 && Recebido pelo Sistema de Registro de Eventos, com vinculação do evento no respetivo MDFe
         oRecepcaoEvento.GravarXmlDistribuicao("d:\testenfe") &&Grava o XML de distribuição
      ELSE
         * Foi rejeitado, fazer devidos tratamentos	  
      ENDIF

   CATCH TO oErro
    * Excecao do FOXPRO
	* Mais sobre excecao em FOXPRO
	* http://www.yaldex.com/fox_pro_tutorial/html/2344b71b-14c0-4125-b001-b5fbb7bd1f05.htm
	
	  MESSAGEBOX("FOXPRO - ErrorCode: " + ALLTRIM(STR(oErro.ErrorNo,10))+ " - Message: " + oErro.Message)
	  
    * Excecao do CSHARP
      MESSAGEBOX("CSHARP - ErrorCode: " + ALLTRIM(STR(oExceptionInterop.GetErrorCode(),20)) + " - Message: " + oExceptionInterop.GetMessage())
   ENDTRY
RETURN
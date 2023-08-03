﻿#pragma warning disable CS1591

#if INTEROP
using System.Runtime.InteropServices;
#endif
using System;
using System.Xml.Serialization;
using Unimake.Business.DFe.Servicos;
using System.Collections.Generic;

namespace Unimake.Business.DFe.Xml.EFDReinf
{
#if INTEROP
    [ClassInterface(ClassInterfaceType.AutoDual)]
    [ProgId("Unimake.Business.DFe.Xml.EFDReinf.Reinf")]
    [ComVisible(true)]
#endif

    [Serializable()]
    [XmlRoot("Reinf", Namespace = "http://www.reinf.esocial.gov.br/schemas/evtInfoContribuinte/v2_01_02", IsNullable = false)]
    public class Reinf : XMLBase
    {
        [XmlElement("evtInfoContri")]
        public EvtInfoContri EvtInfoContri { get; set; }

        [XmlElement(ElementName = "Signature", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public Signature Signature { get; set; }
    }

#if INTEROP
    [ClassInterface(ClassInterfaceType.AutoDual)]
    [ProgId("Unimake.Business.DFe.Xml.EFDReinf.EvtInfoContri")]
    [ComVisible(true)]
#endif
    [Serializable()]
    public class EvtInfoContri
    {
        [XmlAttribute(AttributeName = "id", DataType = "token")]
        public string ID { get; set; }

        [XmlElement("ideEvento")]
        public IdeEvento IdeEvento { get; set; }

        [XmlElement("ideContri")]
        public IdeContri IdeContri { get; set; }

        [XmlElement("infoContri")]
        public InfoContri InfoContri { get; set; }
    }

#if INTEROP
    [ClassInterface(ClassInterfaceType.AutoDual)]
    [ProgId("Unimake.Business.DFe.Xml.EFDReinf.IdeEvento")]
    [ComVisible(true)]
#endif
    [Serializable()]
    public class IdeEvento
    {
        [XmlElement("tpAmb")]
        public TipoAmbiente TpAmb { get; set; }

        [XmlElement("procEmi")]
        public ProcessoEmissaoReinf ProcEmi { get; set; }

        [XmlElement("verProc")]
        public string VerProc { get; set; }
    }

#if INTEROP
    [ClassInterface(ClassInterfaceType.AutoDual)]
    [ProgId("Unimake.Business.DFe.Xml.EFDReinf.IdeContri")]
    [ComVisible(true)]
#endif
    [Serializable()]
    public class IdeContri
    {
        [XmlElement("tpInsc")]
        public TiposInscricao TpInsc { get; set; }

        [XmlElement("nrInsc")]
        public string NrInsc { get; set; }
    }

#if INTEROP
    [ClassInterface(ClassInterfaceType.AutoDual)]
    [ProgId("Unimake.Business.DFe.Xml.EFDReinf.InfoContri")]
    [ComVisible(true)]
#endif
    [Serializable()]
    public class InfoContri
    {
        [XmlElement("inclusao")]
        public Inclusao Inclusao { get; set; }
    }

#if INTEROP
    [ClassInterface(ClassInterfaceType.AutoDual)]
    [ProgId("Unimake.Business.DFe.Xml.EFDReinf.Inclusao")]
    [ComVisible(true)]
#endif
    [Serializable()]
    public class Inclusao
    {
        [XmlElement("idePeriodo")]
        public IdePeriodo IdePeriodo { get; set; }

        [XmlElement("infoCadastro")]
        public InfoCadastro InfoCadastro { get; set; }
    }

#if INTEROP
    [ClassInterface(ClassInterfaceType.AutoDual)]
    [ProgId("Unimake.Business.DFe.Xml.EFDReinf.IdePeriodo")]
    [ComVisible(true)]
#endif
    [Serializable()]
    public class IdePeriodo
    {
        [XmlElement("iniValid")]
        public string IniValid { get; set; }

        [XmlElement("fimValid")]
        public string FimValid { get; set; }

        #region ShouldSerialize

        public bool ShouldSerializeFimValid() => !string.IsNullOrEmpty(FimValid);

        #endregion
    }

#if INTEROP
    [ClassInterface(ClassInterfaceType.AutoDual)]
    [ProgId("Unimake.Business.DFe.Xml.EFDReinf.InfoCadastro")]
    [ComVisible(true)]
#endif
    [Serializable()]
    public class InfoCadastro
    {
        [XmlElement("classTrib")]
        public ClassificacaoTributaria ClassTrib { get; set; }

        [XmlElement("indEscrituracao")]
        public IndicativoEscrituracao IndEscrituracao { get; set; }

        [XmlElement("indDesoneracao")]
        public IndicativoDesoneracao IndDesoneracao { get; set; }

        [XmlElement("indAcordoIsenMulta")]
        public IndicativoIsencaoMulta IndAcordoIsenMulta { get; set; }


        [XmlElement("indSitPJ")]
#if INTEROP
        public IndicativoSituacaoPJ IndSitPJ { get; set; } = (IndicativoSituacaoPJ)(-1);
#else
         public IndicativoSituacaoPJ? IndSitPJ { get; set; }
#endif

        [XmlElement("indUniao")]
        public IndicativoUniao IndUniao { get; set; }

        [XmlIgnore]
#if INTEROP
        public DateTime DtTransfFinsLucr { get; set; }
#else
        public DateTimeOffset DtTransfFinsLucr { get; set; }
#endif

        [XmlElement("dtTransfFinsLucr")]
        public string DtTransfFinsLucrField
        {
            get => DtTransfFinsLucr.ToString("yyyy-MM-dd");
#if INTEROP
            set => DtTransfFinsLucr = DateTime.Parse(value);
#else
            set => DtTransfFinsLucr = DateTimeOffset.Parse(value);
#endif
        }

        [XmlIgnore]
#if INTEROP
        public DateTime DtObito { get; set; }
#else
        public DateTimeOffset DtObito { get; set; }
#endif

        [XmlElement("dtObito")]
        public string DtObitoField
        {
            get => DtObito.ToString("yyyy-MM-dd");
#if INTEROP
            set => DtObito = DateTime.Parse(value);
#else
            set => DtObito = DateTimeOffset.Parse(value);
#endif
        }

        [XmlElement("contato")]
        public Contato Contato { get; set; }

        [XmlElement("softHouse")]
        public List<SoftHouse> SoftHouse { get; set; }

        [XmlElement("infoEFR")]
        public InfoEFR InfoEFR { get; set; }

        #region ShouldSerialize

        public bool ShouldSerializeDtObitoField() => DtObito > DateTime.MinValue;

        public bool ShouldSerializeDtTransfFinsLucrField() => DtTransfFinsLucr > DateTime.MinValue;
#if INTEROP
        public bool ShouldSerializeIndSitPJ() => IndSitPJ != (IndicativoSituacaoPJ)(-1);
#else
        public bool ShouldSerializeIndSitPJ() => IndSitPJ != null;
#endif
        #endregion
    }

#if INTEROP
    [ClassInterface(ClassInterfaceType.AutoDual)]
    [ProgId("Unimake.Business.DFe.Xml.EFDReinf.Contato")]
    [ComVisible(true)]
#endif
    [Serializable()]
    public class Contato
    {
        //precisa de mascara? (⊙_⊙)？

        [XmlElement("nmCtt")]
        public string NmCtt { get; set; }

        [XmlElement("cpfCtt")]
        public string CpfCtt { get; set; }

        [XmlElement("foneFixo")]
        public string FoneFixo { get; set; }

        [XmlElement("foneCel1")]
        public string FoneCel1 { get; set; }

        [XmlElement("email")]
        public string Email { get; set; }
    }

#if INTEROP
    [ClassInterface(ClassInterfaceType.AutoDual)]
    [ProgId("Unimake.Business.DFe.Xml.EFDReinf.SoftHouse")]
    [ComVisible(true)]
#endif
    [Serializable()]
    public class SoftHouse
    {
        [XmlElement("cnpjSoftHouse")]
        public string CnpjSoftHouse { get; set; }

        [XmlElement("nmRazao")]
        public string NmRazao { get; set; }

        [XmlElement("nmCont")]
        public string NmCont { get; set; }

        [XmlElement("telefone")]
        public string Telefone { get; set; }

        [XmlElement("email")]
        public string Email { get; set; }
    }

#if INTEROP
    [ClassInterface(ClassInterfaceType.AutoDual)]
    [ProgId("Unimake.Business.DFe.Xml.EFDReinf.InfoEFR")]
    [ComVisible(true)]
#endif
    [Serializable()]
    public class InfoEFR
    {
        [XmlElement("ideEFR")]
        public string IdeEFR { get; set; }

        [XmlElement("cnpjEFR")]
        public string CnpjEFR { get; set; }
    }
}
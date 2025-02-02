﻿using DevExpress.Web.Bootstrap;
using PortalClientes.Clases;
using PortalClientes.Objetos;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PortalClientes
{
    public partial class Site1 : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserIdentity"] != null)
            {
                UserIdentity oUI = (UserIdentity)Session["UserIdentity"];
                txtLang.Text = oUI.sIdioma;
                lblNombreUsuario.Text = oUI.sNombre;
            }

            if (txtLang.Text != Utils.Idioma)
            {
                txtLang.Text = Utils.Idioma;
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(Utils.Idioma);
                ArmaFormulario();
            }
            else
            {
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(Utils.Idioma);
                ArmaFormulario();
            }

            if(Utils.Idioma == "es-MX")
            {
                imgIconoEspanol.Visible = true;
                imgIconoEnglish.Visible = false;
            }
            else
            {
                imgIconoEspanol.Visible = false;
                imgIconoEnglish.Visible = true;
            }

            if(Utils.Matriculas != null)
            {
                ArmaMenu();
            }
            

            if (!IsPostBack)
            {
                Session["cambioMatricula"] = Session["cambioMatricula"] != null ? Session["cambioMatricula"] : 0;
                var x = Session["cambioMatricula"];
                CargaMatriculas();  
            }
        }

        protected void lblIdiomaEspanol_Click(object sender, EventArgs e)
        {
            Utils.Idioma = "es-MX";
            string strUrl = string.Empty;
            strUrl = Page.Request.Url.ToString().Replace("192.168.1.250", "www.alesuite.com");
            strUrl = Page.Request.Url.ToString().Replace("portal", "alesuite.com");
            Page.Response.Redirect(strUrl, true);
        }

        protected void lblIdiomaEnglish_Click(object sender, EventArgs e)
        {
            Utils.Idioma = "en-US";
            string strUrl = string.Empty;
            strUrl = Page.Request.Url.ToString().Replace("192.168.1.250", "www.alesuite.com");
            strUrl = Page.Request.Url.ToString().Replace("portal", "alesuite.com");
            Page.Response.Redirect(strUrl, true);
        }

        private void ArmaFormulario()
        {
            lblUsuario.Text = "  " + Utils.NombreUsuario;
            lblMatricula.Text = "  " + Properties.Resources.MatriculasMenu;
            lblAdminMiCuenta.Text = "  " + Properties.Resources.AdministrarCuenta;
            lblSalir.Text = "  " + Properties.Resources.CerrarSesion;
        }

        private void ArmaMenu()
        {
            List<MenuDinamico> olst = ObtieneMenu();
            string sHtml = "<ul class='nav side-menu'> ";
            string sliHtml = "";

            foreach (MenuDinamico oMenu in olst)
            {
                string sNombre = Utils.Idioma == "es-MX" || Utils.Idioma == string.Empty ? oMenu.nombreESP : oMenu.nombreUSD;
                if(sNombre == "Usuarios" || sNombre == "Users")
                {
                    sliHtml += "<li class='item'><a href = '" + oMenu.urlPage + "'><i class='" + oMenu.style + "'></i>" + sNombre + "</a></li>";
                }
                else
                {
                    sHtml += "<li class='item'><a href = '" + oMenu.urlPage + "'><i class='" + oMenu.style + "'></i>" + sNombre + "</a></li>";
                }             
            }
            sHtml += sliHtml;
            sHtml += " </ul>";

            divMenu.InnerHtml = sHtml;
        }

        private List<MenuDinamico> ObtieneMenu()
        {
            try
            {
                List<MenuDinamico> olstFinal = new List<MenuDinamico>();
                List<ModulosUsuario> olst = new List<ModulosUsuario>();
                JavaScriptSerializer ser = new JavaScriptSerializer();
                TokenWS oToken = Utils.ObtieneToken;
                requestIdUsuario oRe = new requestIdUsuario();
                oRe.idUsuario = Utils.GetIdEmpUsuario;

                var client = new RestClient(Helper.US_UrlConsultaModulosPorUsuario);
                var request = new RestRequest(Method.POST);
                request.AddHeader("Authorization", oToken.token);
                request.AddJsonBody(oRe);

                IRestResponse response = client.Execute(request);
                var resp = response.Content;
                olst = ser.Deserialize<List<ModulosUsuario>>(resp);

                MenuDinamico oMenu0 = new MenuDinamico();
                oMenu0.urlPage = "frmDefault.aspx";
                oMenu0.style = "fa fa-home";
                oMenu0.nombreESP = "Inicio";
                oMenu0.nombreUSD = "Home";

                //olstFinal.Add(oMenu0);

                foreach (ModulosUsuario oMenu in olst)
                {
                    if (oMenu.sts == 1)
                    {
                        MenuDinamico oM = new MenuDinamico();
                        oM.urlPage = oMenu.urlPage;
                        oM.style = oMenu.icono;
                        oM.nombreESP = oMenu.nombreESP;
                        oM.nombreUSD = oMenu.nombreENG;

                        olstFinal.Add(oM);
                    }
                }

                return olstFinal;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void CargaMatriculas()
        {
            
            if (Session["UserIdentity"] != null)
            {
                UserIdentity oU = (UserIdentity)Session["UserIdentity"];
                if (oU != null)
                {

                    BootstrapMenuItem oMenuMats = new BootstrapMenuItem();

                    if (oU.lsMatriculas != null)
                    {
                        foreach (string sMat in oU.lsMatriculas)
                        {
                            BootstrapMenuItem oItem = new BootstrapMenuItem();
                            oItem.Text = sMat;
                            oItem.GroupName = "Group4";
                            oItem.IconCssClass = "fa fa-caret-right";
                            oItem.CssClass = "icon_left";
                            MenuMatriculas.Items.Add(oItem);
                        }
                    }
                    foreach (var item in ObtieneMatriculasContrato())
                    {
                        if (item.matriculaDefault == 1 && (int)Session["cambioMatricula"] == 0)
                        {
                            Utils.ClaveContrato = item.claveCliente;
                            Utils.NombreCliente = item.nombre;
                            lblContratoCliente.Text = item.claveCliente;
                            lblAeronave.Text = item.matricula;
                            lblAeronaveLat.Text = item.matricula;
                            Utils.MatriculaActual = item.matricula;
                            break;
                        }
                        else
                        {
                            lblAeronave.Text = Utils.MatriculaActual;
                            lblAeronaveLat.Text = Utils.MatriculaActual;
                            lblContratoCliente.Text = Utils.ClaveContrato;
                        }
                    }
                }
            }
        }

        protected void MenuMatriculas_ItemClick(object source, DevExpress.Web.Bootstrap.BootstrapMenuItemEventArgs e)
        {
            if (e.Item.GroupName == "Group1")
            {
                Session["UserIdentity"] = null;
                Response.Redirect("~/frmLogin.aspx");
            }

            if (e.Item.GroupName == "Group3")
            {
                Response.Redirect("~/Views/frmEditusuario.aspx");
            }

            string[] pathList = HttpContext.Current.Request.Url.Segments;
            var url = HttpContext.Current.Request.Url.AbsoluteUri;
            string path = pathList[2];

            if (e.Item.GroupName == "Group4")
            {
                if(Utils.MatriculaActual != e.Item.Text && e.Item.Text != "Matrículas ⏷")
                {
                    Utils.MatriculaActual = e.Item.Text;
                    lblAeronave.Text = e.Item.Text;
                    lblAeronaveLat.Text = e.Item.Text;
                    Session["cambioMatricula"] = 1;
                }
                else
                {
                    lblAeronave.Text = Utils.MatriculaActual;
                    lblAeronaveLat.Text = Utils.MatriculaActual;
                }

                foreach (var item in ObtieneMatriculasContrato())
                {
                    if (item.matricula == Utils.MatriculaActual)
                    {
                        lblContratoCliente.Text = item.claveCliente;
                        Utils.ClaveContrato = item.claveCliente;
                        Utils.NombreCliente = item.nombre;
                    }
                }
            }
            string strUrl = string.Empty;
            strUrl = Page.Request.Url.ToString().Replace("192.168.1.250", "alesuite.com");
            strUrl = Page.Request.Url.ToString().Replace("portal", "alesuite.com");
            Page.Response.Redirect(strUrl, false);
            Context.ApplicationInstance.CompleteRequest();
        }

        protected void btnSalir_Click(object sender, EventArgs e)
        {
            Session["UserIdentity"] = null;
            Response.Redirect("~/frmLogin.aspx");
        }

        protected void btnAdminCuenta_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Views/frmEditusuario.aspx");
        }

        private List<MatriculasContratoUsuario> ObtieneMatriculasContrato()
        {
            List<MatriculasContratoUsuario> olst = new List<MatriculasContratoUsuario>();
            JavaScriptSerializer ser = new JavaScriptSerializer();
            TokenWS oToken = Utils.ObtieneToken;
            requestIdUsuario oRe = new requestIdUsuario();
            oRe.idUsuario = Utils.GetIdEmpUsuario;

            var client = new RestClient(Helper.US_UrlObtieneMatriculasContratos);
            var request = new RestRequest(Method.POST);
            request.AddHeader("Authorization", oToken.token);
            request.AddJsonBody(oRe);

            IRestResponse response = client.Execute(request);
            var resp = response.Content;
            olst = ser.Deserialize<List<MatriculasContratoUsuario>>(resp);
            Session["LstMatriculas"] = olst;

            return olst;
        }
    }
}
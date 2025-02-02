﻿using PortalClientes.Objetos;
using PortalClientes.Clases;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json;
using System.Web.Services;
using DevExpress.Web;
using NucleoBase.Core;
using System.Globalization;
using System.Drawing;
using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using System.Threading;
using System.Text;
using PortalClientes.DomainModel;
using System.Data;

//using OfficeOpenXml;

namespace PortalClientes.Views
{

    class Meses
    {
        string mes { set; get; }
    }
    public partial class frmTransacciones : System.Web.UI.Page
    {

        #region EVENTOS
        protected void Page_Load(object sender, EventArgs e)
        {
            //this.Page.Form.Attributes.Add("enctype", "multipart/form-data");

            Session["RefUrl"] = Request.Url.ToString();
            if (System.Web.HttpContext.Current.Session["UserIdentity"] == null)
            {
                Response.Redirect("~/Views/frmFinconexion2.aspx");
            }

            var tipo = Convert.ToInt32(Session["tipoTransaccion"]);
            var od = Convert.ToInt32(Session["origenData"]);
            var descripcion = (string)Session["descripcion"];
            //oPresenter = new Transacciones_Presenter(this, new DBTransacciones());

            string Smatricula = "";

            Session["title"] = GenerateTitle(true, tipo, descripcion);
            Session["titleFile"] = GenerateTitle(false, tipo, descripcion);

            TextBox milabel = (TextBox)this.Master.FindControl("txtLang");
            if (milabel.Text != Utils.Idioma && milabel.Text != string.Empty)
            {
                Utils.Idioma = milabel.Text;
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(Utils.Idioma);
                ArmarTransacciones();
            }
            else
            {
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(Utils.Idioma);
                ArmarTransacciones();
            }

            if (!IsPostBack)
            {
                if (Session["data"] != null)
                {
                    tipo = Convert.ToInt32(Session["tipoTransaccion"]);
                    od = Convert.ToInt32(Session["origenData"]);
                    btnExcelUSD.Visible = false;
                    pnlusd.Visible = false;

                    Transacciones transacciones = new Transacciones();
                    if (tipo == 1)
                    {
                        transacciones.gastos = (List<gvGastos>)Session["data"];
                        LlenarGV(transacciones, tipo);
                    }

                    else if (tipo == 2)
                    {
                        transacciones.gastosAe = (List<gvGastosAeropuerto>)Session["data"];
                        LlenarGV(transacciones, tipo);
                    }

                    else if (tipo == 3)
                    {
                        transacciones.gastosProv = (List<gvGastosProveedor>)Session["data"];
                        LlenarGV(transacciones, tipo);
                    }

                    else if (tipo == 4)
                    {
                        transacciones.vuelos = (List<vuelo>)Session["data"];
                        LlenarGV(transacciones, tipo);
                    }

                    else if (tipo == 5)
                    {
                        transacciones.promedioPax = (List<gvPromedioPax>)Session["data"];
                        LlenarGV(transacciones, tipo);
                    }

                    else if (tipo == 6)
                    {
                        transacciones.promedioCosto = (List<gvPromedioCosto>)Session["data"];
                        LlenarGV(transacciones, tipo);
                    }

                    else if (tipo == 7)
                    {
                        transacciones.horasVoladas = (List<gvhorasVoladas>)Session["data"];
                        LlenarGV(transacciones, tipo);
                    }

                    else if (tipo == 8)
                    {
                        transacciones.numeroVuelos = (List<gvnoVuelos>)Session["data"];
                        LlenarGV(transacciones, tipo);
                    }

                    else if (tipo == 9)
                    {
                        transacciones.costosFijosVariable = (List<gvCostosFV>)Session["data"];
                        LlenarGV(transacciones, tipo);
                    }

                    else if (tipo == 10)
                    {
                        transacciones.gastosTotales = (List<gvGastosT>)Session["data"];
                        LlenarGV(transacciones, tipo);
                    }

                    else if (tipo == 11)
                    {
                        transacciones.costosHoraVuelo = (List<gvCostosH>)Session["data"];
                        LlenarGV(transacciones, tipo);
                    }

                    else if (tipo == 12)
                    {
                        transacciones.costosFijosVariableHora = (List<gvCostosFVH>)Session["data"];
                        LlenarGV(transacciones, tipo);
                    }

                    else if (tipo == 13)
                    {
                        transacciones.detalleEdoCuenta = (List<detalleEdoCta>)Session["data"];
                        LlenarGV(transacciones, tipo);
                        LlenarGVUSD(transacciones, tipo);
                        btnExcelUSD.Visible = true;
                        pnlusd.Visible = true;
                        btnRegresaEdoCta.Visible = true;
                    }

                    else if (tipo == 14)
                    {
                        transacciones.detGastos = (List<gvDetGastos>)Session["data"];
                        LlenarGV(transacciones, tipo);
                    }

                    if (od == 1)
                    {
                        btnRegresarMeEs.Visible = false;
                        btnRegresarMeEsEng.Visible = false;
                        btnRegresarDash.Visible = true;
                        btnRegresaEdoCta.Visible = false;
                        btnRegresaEdoCtaEng.Visible = false;
                    }
                    else if (od == 2)
                    {
                        btnRegresarDash.Visible = false;
                        btnRegresaEdoCta.Visible = false;
                        btnRegresaEdoCtaEng.Visible = false;

                        if (Utils.Idioma == "es-MX")
                        {
                            btnRegresarMeEs.Visible = true;
                            btnRegresarMeEsEng.Visible = false;
                        }
                        else
                        {
                            btnRegresarMeEs.Visible = false;
                            btnRegresarMeEsEng.Visible = true;
                        }
                    }
                    else if (od == 3)
                    {
                        btnRegresarDash.Visible = false;
                        btnRegresarMeEs.Visible = false;
                        btnRegresarMeEsEng.Visible = false;

                        if (Utils.Idioma == "es-MX")
                        {
                            btnRegresaEdoCta.Visible = true;
                            btnRegresaEdoCtaEng.Visible = false;
                        }
                        else
                        {
                            btnRegresaEdoCta.Visible = false;
                            btnRegresaEdoCtaEng.Visible = true;
                        }
                    }
                } 
            }
        }
        protected void gvGastos_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
            {
                var tipo = Convert.ToInt32(Session["tipoTransaccion"]);

                if (tipo == 1)
                {
                    e.Row.Cells[0].Text = Properties.Resources.TabTran_Mes;
                    e.Row.Cells[1].Text = Properties.Resources.TabTran_Rubro;
                    e.Row.Cells[2].Text = Properties.Resources.TabTran_Total;
                    e.Row.Cells[3].Text = Properties.Resources.TabTran_Fecha;
                    e.Row.Cells[4].Text = Properties.Resources.TabTran_Categoria;
                    e.Row.Cells[5].Text = Properties.Resources.TabTran_TGasto;
                    e.Row.Cells[6].Text = Properties.Resources.TabTran_Comentario;
                }

                else if (tipo == 2)
                {
                    e.Row.Cells[0].Text = Properties.Resources.TabTran_Mes;
                    e.Row.Cells[1].Text = Properties.Resources.TabTran_Rubro;
                    e.Row.Cells[2].Text = Properties.Resources.TabTran_Total;
                    e.Row.Cells[3].Text = Properties.Resources.TabTran_Anio;
                    e.Row.Cells[4].Text = Properties.Resources.TabTran_NoPierna;
                    e.Row.Cells[5].Text = Properties.Resources.TabTran_Proveedor;
                    e.Row.Cells[6].Text = Properties.Resources.TabTran_Categoria;
                    e.Row.Cells[7].Text = Properties.Resources.TabTran_Origen;
                    e.Row.Cells[8].Text = Properties.Resources.TabTran_Destino;
                }

                else if (tipo == 3)
                {
                    e.Row.Cells[0].Text = Properties.Resources.TabTran_Mes;
                    e.Row.Cells[1].Text = Properties.Resources.TabTran_Rubro;
                    e.Row.Cells[2].Text = Properties.Resources.TabTran_Total;
                    e.Row.Cells[3].Text = Properties.Resources.TabTran_Anio;
                    e.Row.Cells[4].Text = Properties.Resources.TabTran_NoPierna;
                    e.Row.Cells[5].Text = Properties.Resources.TabTran_Proveedor;
                    e.Row.Cells[6].Text = Properties.Resources.TabTran_Categoria;
                    e.Row.Cells[7].Text = Properties.Resources.TabTran_Origen;
                    e.Row.Cells[8].Text = Properties.Resources.TabTran_Destino;
                }

                else if (tipo == 4)
                {
                    e.Row.Cells[0].Text = Properties.Resources.TabTran_Mes;
                    e.Row.Cells[1].Text = Properties.Resources.TabTran_Anio;
                    e.Row.Cells[2].Text = Properties.Resources.TabTran_Origen;
                    e.Row.Cells[3].Text = Properties.Resources.TabTran_Destino;
                    e.Row.Cells[4].Text = Properties.Resources.TabTran_IVuelo; 
                    e.Row.Cells[5].Text = Properties.Resources.TabTran_FVuelo;
                    e.Row.Cells[6].Text = Properties.Resources.TabTran_TVuelo;
                    e.Row.Cells[7].Text = Properties.Resources.TabTran_Categoria;
                }

                else if (tipo == 5)
                {
                    e.Row.Cells[0].Text = Properties.Resources.TabTran_Mes;
                    e.Row.Cells[1].Text = Properties.Resources.TabTran_Anio;
                    e.Row.Cells[2].Text = Properties.Resources.TabTran_Origen;
                    e.Row.Cells[3].Text = Properties.Resources.TabTran_Destino;
                    e.Row.Cells[4].Text = Properties.Resources.TabTran_IVuelo;
                    e.Row.Cells[5].Text = Properties.Resources.TabTran_FVuelo;
                    e.Row.Cells[6].Text = Properties.Resources.TabTran_TVuelo;
                    e.Row.Cells[7].Text = Properties.Resources.TabTran_CPax;
                    e.Row.Cells[8].Text = Properties.Resources.TabTran_Contrato;
                    e.Row.Cells[9].Text = Properties.Resources.TabTran_Cliente;
                }

                else if (tipo == 6)
                {
                    e.Row.Cells[0].Text = Properties.Resources.TabTran_Mes;
                    e.Row.Cells[1].Text = Properties.Resources.TabTran_Anio;
                    e.Row.Cells[2].Text = Properties.Resources.TabTran_Rubro;
                    e.Row.Cells[3].Text = Properties.Resources.TabTran_Importe;
                    e.Row.Cells[4].Text = Properties.Resources.TabTran_Categoria;
                    e.Row.Cells[5].Text = Properties.Resources.TabTran_TGasto;
                    e.Row.Cells[6].Text = Properties.Resources.TabTran_Comentario;
                }

                else if (tipo == 7) {
                    e.Row.Cells[0].Text = Properties.Resources.TabTran_Mes;
                    e.Row.Cells[1].Text = Properties.Resources.TabTran_Anio;
                    e.Row.Cells[2].Text = Properties.Resources.TabTran_Origen;
                    e.Row.Cells[3].Text = Properties.Resources.TabTran_Destino;
                    e.Row.Cells[4].Text = Properties.Resources.TabTran_IVuelo;
                    e.Row.Cells[5].Text = Properties.Resources.TabTran_FVuelo;
                    e.Row.Cells[6].Text = Properties.Resources.TabTran_TVuelo;
                    e.Row.Cells[7].Text = Properties.Resources.TabTran_CPax;
                    e.Row.Cells[8].Text = Properties.Resources.TabTran_Contrato;
                    e.Row.Cells[9].Text = Properties.Resources.TabTran_Cliente;
                }

                else if (tipo == 8)
                {
                    e.Row.Cells[0].Text = Properties.Resources.TabTran_Mes;
                    e.Row.Cells[1].Text = Properties.Resources.TabTran_Anio;
                    e.Row.Cells[2].Text = Properties.Resources.TabTran_Origen;
                    e.Row.Cells[3].Text = Properties.Resources.TabTran_Destino;
                    e.Row.Cells[4].Text = Properties.Resources.TabTran_IVuelo;
                    e.Row.Cells[5].Text = Properties.Resources.TabTran_FVuelo;
                    e.Row.Cells[6].Text = Properties.Resources.TabTran_TVuelo;
                    e.Row.Cells[7].Text = Properties.Resources.TabTran_CPax;
                    e.Row.Cells[8].Text = Properties.Resources.TabTran_Contrato;
                    e.Row.Cells[9].Text = Properties.Resources.TabTran_Cliente;
                }

                else if (tipo == 9)
                {
                    e.Row.Cells[0].Text = Properties.Resources.TabTran_Mes;
                    e.Row.Cells[1].Text = Properties.Resources.TabTran_Anio;
                    e.Row.Cells[2].Text = Properties.Resources.TabTran_Rubro;
                    e.Row.Cells[3].Text = Properties.Resources.TabTran_Importe;
                    e.Row.Cells[4].Text = Properties.Resources.TabTran_Categoria;
                    e.Row.Cells[5].Text = Properties.Resources.TabTran_TGasto;
                    e.Row.Cells[6].Text = Properties.Resources.TabTran_Comentario;
                }

                else if (tipo == 10)
                {
                    e.Row.Cells[0].Text = Properties.Resources.TabTran_Mes;
                    e.Row.Cells[1].Text = Properties.Resources.TabTran_Anio;
                    e.Row.Cells[2].Text = Properties.Resources.TabTran_Rubro;
                    e.Row.Cells[3].Text = Properties.Resources.TabTran_Importe;
                    e.Row.Cells[4].Text = Properties.Resources.TabTran_Categoria;
                    e.Row.Cells[5].Text = Properties.Resources.TabTran_Comentario;
                }

                else if (tipo == 11)
                {
                    e.Row.Cells[0].Text = Properties.Resources.TabTran_Mes;
                    e.Row.Cells[1].Text = Properties.Resources.TabTran_Anio;
                    e.Row.Cells[2].Text = Properties.Resources.TabTran_Rubro;
                    e.Row.Cells[3].Text = Properties.Resources.TabTran_Importe;
                    e.Row.Cells[4].Text = Properties.Resources.TabTran_Categoria;
                    e.Row.Cells[5].Text = Properties.Resources.TabTran_Comentario;
                }

                else if (tipo == 12)
                {
                    e.Row.Cells[0].Text = Properties.Resources.TabTran_Mes;
                    e.Row.Cells[1].Text = Properties.Resources.TabTran_Anio;
                    e.Row.Cells[2].Text = Properties.Resources.TabTran_Rubro;
                    e.Row.Cells[3].Text = Properties.Resources.TabTran_Importe;
                    e.Row.Cells[4].Text = Properties.Resources.TabTran_Categoria;
                    e.Row.Cells[5].Text = Properties.Resources.TabTran_Comentario;
                }

                else if (tipo == 13)
                {
                    e.Row.Cells[0].Text = Properties.Resources.TabTran_Mes;
                    e.Row.Cells[1].Text = Properties.Resources.TabTran_TMoneda;
                    e.Row.Cells[2].Text = Properties.Resources.TabTran_Fecha;
                    e.Row.Cells[3].Text = Properties.Resources.TabTran_NoReferencia;
                    e.Row.Cells[4].Text = Properties.Resources.TabTran_TGasto;
                    e.Row.Cells[5].Text = Properties.Resources.TabTran_Concepto;
                    e.Row.Cells[6].Text = Properties.Resources.TabTran_Detalle;
                    e.Row.Cells[7].Text = Properties.Resources.TabTran_Proveedor;
                    e.Row.Cells[8].Text = Properties.Resources.TabTran_Importe;
                }

                else if (tipo == 14)
                {
                    e.Row.Cells[0].Text = Properties.Resources.TabTran_Mes;
                    e.Row.Cells[1].Text = Properties.Resources.TabTran_Anio;
                    e.Row.Cells[2].Text = Properties.Resources.TabTran_Rubro;
                    e.Row.Cells[3].Text = Properties.Resources.TabTran_Importe;
                    e.Row.Cells[4].Text = Properties.Resources.TabTran_Categoria;
                    e.Row.Cells[5].Text = Properties.Resources.TabTran_TGasto;
                    e.Row.Cells[6].Text = Properties.Resources.TabTran_Comentario;
                }

            }
        }

        protected void gvGastos_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            var tipo = Convert.ToInt32(Session["tipoTransaccion"]);
            Transacciones transacciones = new Transacciones();
            gvGastos.Columns.Clear();
            gvGastos.PageIndex = e.NewPageIndex;
            if (tipo == 1)
            {
                transacciones.gastos = (List<gvGastos>)Session["data"];
                LlenarGV(transacciones, tipo);
            }

            else if (tipo == 2)
            {
                transacciones.gastosAe = (List<gvGastosAeropuerto>)Session["data"];
                LlenarGV(transacciones, tipo);
            }

            else if (tipo == 3)
            {
                transacciones.gastosProv = (List<gvGastosProveedor>)Session["data"];
                LlenarGV(transacciones, tipo);
            }

            else if (tipo == 4)
            {
                transacciones.gastos = (List<gvGastos>)Session["data"];
                LlenarGV(transacciones, tipo);
            }

            else if (tipo == 5)
            {
                transacciones.promedioPax = (List<gvPromedioPax>)Session["data"];
                LlenarGV(transacciones, tipo);
            }

            else if (tipo == 6)
            {
                transacciones.promedioCosto = (List<gvPromedioCosto>)Session["data"];
                LlenarGV(transacciones, tipo);
            }

            else if (tipo == 7)
            {
                transacciones.horasVoladas = (List<gvhorasVoladas>)Session["data"];
                LlenarGV(transacciones, tipo);
            }

            else if (tipo == 8)
            {
                transacciones.numeroVuelos = (List<gvnoVuelos>)Session["data"];
                LlenarGV(transacciones, tipo);
            }

            else if (tipo == 9)
            {
                transacciones.costosFijosVariable = (List<gvCostosFV>)Session["data"];
                LlenarGV(transacciones, tipo);
            }

            else if (tipo == 10)
            {
                transacciones.gastosTotales = (List<gvGastosT>)Session["data"];
                LlenarGV(transacciones, tipo);
            }

            else if (tipo == 11)
            {
                transacciones.costosHoraVuelo = (List<gvCostosH>)Session["data"];
                LlenarGV(transacciones, tipo);
            }

            else if (tipo == 12)
            {
                transacciones.costosFijosVariableHora = (List<gvCostosFVH>)Session["data"];
                LlenarGV(transacciones, tipo);
            }

            else if (tipo == 13)
            {
                transacciones.detalleEdoCuenta = (List<detalleEdoCta>)Session["data"];
                LlenarGV(transacciones, tipo);
            }

            else if (tipo == 14)
            {
                transacciones.detGastos = (List<gvDetGastos>)Session["data"];
                LlenarGV(transacciones, tipo);
            }

        }

        protected void gvGastosUSD_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            var tipo = Convert.ToInt32(Session["tipoTransaccion"]);
            Transacciones transacciones = new Transacciones();
            gvGastosUSD.PageIndex = e.NewPageIndex;

            if (tipo == 13)
            {
                transacciones.detalleEdoCuenta = (List<detalleEdoCta>)Session["data"];
                LlenarGVUSD(transacciones, tipo);
            }
        }

        protected void gvGastosRef_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
            {
                var tipo = Convert.ToInt32(Session["tipoTransaccion"]);

                if (tipo == 13)
                {
                    e.Row.Cells[0].Text = Properties.Resources.TabTran_Mes;
                    e.Row.Cells[1].Text = Properties.Resources.TabTran_TMoneda;
                    e.Row.Cells[2].Text = Properties.Resources.TabTran_Fecha;
                    e.Row.Cells[3].Text = Properties.Resources.TabTran_NoReferencia;
                    e.Row.Cells[4].Text = Properties.Resources.TabTran_TGasto;
                    e.Row.Cells[5].Text = Properties.Resources.TabTran_Concepto;
                    e.Row.Cells[6].Text = Properties.Resources.TabTran_Detalle;
                    e.Row.Cells[7].Text = Properties.Resources.TabTran_Proveedor;
                    e.Row.Cells[8].Text = Properties.Resources.TabTran_Importe;
                }
            }
        }

        protected void gvGastosUSD_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
            {
                var tipo = Convert.ToInt32(Session["tipoTransaccion"]);

                if (tipo == 13)
                {
                    e.Row.Cells[0].Text = Properties.Resources.TabTran_Mes;
                    e.Row.Cells[1].Text = Properties.Resources.TabTran_TMoneda;
                    e.Row.Cells[2].Text = Properties.Resources.TabTran_Fecha;
                    e.Row.Cells[3].Text = Properties.Resources.TabTran_NoReferencia;
                    e.Row.Cells[4].Text = Properties.Resources.TabTran_TGasto;
                    e.Row.Cells[5].Text = Properties.Resources.TabTran_Concepto;
                    e.Row.Cells[6].Text = Properties.Resources.TabTran_Detalle;
                    e.Row.Cells[7].Text = Properties.Resources.TabTran_Proveedor;
                    e.Row.Cells[8].Text = Properties.Resources.TabTran_Importe;
                }
            }
        }

        protected void gvGastosRef_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try
            {
                var tipo = Convert.ToInt32(Session["tipoTransaccion"]);
                Transacciones transacciones = new Transacciones();
                gvGastosRef.PageIndex = e.NewPageIndex;

                if (tipo == 13)
                {
                    transacciones.detalleEdoCuenta = (List<detalleEdoCta>)Session["data"];
                    LlenarGV(transacciones, tipo);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected void gvGastosRef_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                switch (e.CommandName)
                {
                    case "ViewReference":
                        int index = e.CommandArgument.I();
                        Label lblReferencia = (Label)gvGastosRef.Rows[index].FindControl("lblReferenciaPesos");
                        Label lblMesAnio = (Label)gvGastosRef.Rows[index].FindControl("lblMesAnio");

                        string[] sPeriodo = null;
                        sPeriodo = lblMesAnio.Text.Split(' ');

                        string sReferencia = lblReferencia.Text;
                        string sUrl = sReferencia + ".pdf";
                        string sMatricula = Utils.MatriculaActual;
                        int iAnio = sPeriodo[1].I();
                        string sMes = sPeriodo[0].S();
                        int iMes = ObtenerNumeroMes(sMes);

                        DataTable dt = new DBTransacciones().DBGetDetalleReferencia(sReferencia, sMatricula, iAnio.S(), iMes.S());
                        if (dt.Rows.Count > 0)
                        {
                            string sRuta = ArmaRutaComprobante(sReferencia, sPeriodo);

                            if (File.Exists(sRuta + sUrl))
                            {
                                ScriptManager.RegisterStartupScript(Page, typeof(Page), "OpenWindow", "window.open('frmVistaPreviaRef.aspx?sRuta=" + sRuta + sUrl + "',this.target, 'width=600,height=600,top=120,left=400,toolbar=no,location=no,status=no,menubar=no');", true);
                            }
                            else
                            {
                                ScriptManager.RegisterStartupScript(Page, typeof(Page), "Mensaje", "alert('No se encontró el archivo, favor de verificar');", true);
                            }
                            lblTotal.Text = Properties.Resources.TabTran_TotalTranMXN;
                            lblPromedio.Text = Properties.Resources.TabTran_TotalTranUSD;
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected void gvGastosUSD_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                switch (e.CommandName)
                {
                    case "ViewReference":
                        int index = e.CommandArgument.I();
                        Label lblReferencia = (Label)gvGastosUSD.Rows[index].FindControl("lblReferenciaUSD");
                        Label lblMesAnio = (Label)gvGastosUSD.Rows[index].FindControl("lblMesAnio");

                        string[] sPeriodo = null;
                        sPeriodo = lblMesAnio.Text.Split(' ');

                        string sReferencia = lblReferencia.Text;
                        string sUrl = sReferencia + ".pdf";
                        string sMatricula = Utils.MatriculaActual;
                        int iAnio = sPeriodo[1].I();
                        string sMes = sPeriodo[0].S();
                        int iMes = ObtenerNumeroMes(sMes);

                        DataTable dt = new DBTransacciones().DBGetDetalleReferencia(sReferencia, sMatricula, iAnio.S(), iMes.S());
                        if (dt.Rows.Count > 0)
                        {
                            string sRuta = ArmaRutaComprobante(sReferencia, sPeriodo);

                            if (File.Exists(sRuta + sUrl))
                            {
                                ScriptManager.RegisterStartupScript(Page, typeof(Page), "OpenWindow", "window.open('frmVistaPreviaRef.aspx?sRuta=" + sRuta + sUrl + "',this.target, 'width=600,height=600,top=120,left=400,toolbar=no,location=no,status=no,menubar=no');", true);
                            }
                            else
                            {
                                ScriptManager.RegisterStartupScript(Page, typeof(Page), "Mensaje", "alert('No se encontró el archivo, favor de verificar');", true);
                            }
                            lblTotal.Text = Properties.Resources.TabTran_TotalTranMXN;
                            lblPromedio.Text = Properties.Resources.TabTran_TotalTranUSD;
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected void btnExcel_Click(object sender, EventArgs e)
        {
            try
            { 
            exportarExcel();
            }
            catch (Exception ex)
            {
                string strError = ex.Message;
            }
        }

        protected void btnPDF_Click(object sender, EventArgs e)
        {
            try
            { 
            exportarPDF();
            }
            catch (Exception ex)
            {
                string strError = ex.Message;
            }
        }

        public override void VerifyRenderingInServerForm(Control control)
        {
            //return;
        }

        #endregion


        #region METODOS
        private void LlenarGV(Transacciones transacciones, int tipoTransaccion)
        {
            var contMeses = 0;
            var totalTransacciones = 0.0;
            var totalTiempoVuelo = "00:00";
            var TiempoVueloProm = "00:00";
            var promedio = 0.0;
            var totalRegistros = 0;
            var dataOpt = (camposOpcionales)Session["dataOpt"];

            gvGastos.DataSource = null;
            gvGastos.Columns.Clear();

            if (tipoTransaccion == 1)
            {
                totalRegistros = transacciones.gastos.Count();
                contMeses = transacciones.gastos.GroupBy(r => r.mes).Count();
                totalTransacciones = transacciones.gastos.Sum(x => x.Total);
                promedio = totalTransacciones / contMeses;

                BoundField clm = new BoundField();
                clm.DataField = "mes";
                gvGastos.Columns.Add(clm);

                BoundField clm3 = new BoundField();
                clm3.DataField = "Rubro";
                gvGastos.Columns.Add(clm3);

                BoundField clm4 = new BoundField();
                clm4.DataField = "Total";
                clm4.DataFormatString = "{0:C2}";
                gvGastos.Columns.Add(clm4);

                BoundField clm5 = new BoundField();
                clm5.DataField = "Fecha";
                gvGastos.Columns.Add(clm5);

                BoundField clm6 = new BoundField();
                clm6.DataField = "Categoria";
                gvGastos.Columns.Add(clm6);

                BoundField clm7 = new BoundField();
                clm7.DataField = "tipodeGasto";
                gvGastos.Columns.Add(clm7);

                BoundField clm8 = new BoundField();
                clm8.DataField = "comentarios";
                gvGastos.Columns.Add(clm8);

                gvGastos.DataSource = transacciones.gastos.OrderBy(x => x.Fecha).ToList(); 
                gvGastos.DataBind();

                pnlGastos.Visible = true;
                pnlGastosViewRef.Visible = false;

                lblTotalTrasnRes.Text = totalRegistros.S();
                lblTotalRes.Text = string.Format("{0:###,###,###,0.##}", totalTransacciones) + " " + (string)Session["tipoDet"];
                lblPromedioRes.Text = string.Format("{0:###,###,###,0.##}", promedio) + " " + (string)Session["tipoDet"];
            }
            else if (tipoTransaccion == 2)
            {
                totalRegistros = transacciones.gastosAe.Count();
                contMeses = transacciones.gastosAe.GroupBy(r => r.Mes).Count();
                totalTransacciones = transacciones.gastosAe.Sum(x => x.Total);
                promedio = totalTransacciones / contMeses;

                BoundField clm = new BoundField();
                clm.DataField = "mes";
                gvGastos.Columns.Add(clm);

                BoundField clm3 = new BoundField();
                clm3.DataField = "Rubro";
                gvGastos.Columns.Add(clm3);

                BoundField clm4 = new BoundField();
                clm4.DataField = "Total";
                clm4.DataFormatString = "{0:c}";
                gvGastos.Columns.Add(clm4);

                BoundField clm6 = new BoundField();
                clm6.DataField = "Anio";
                gvGastos.Columns.Add(clm6);

                BoundField clm7 = new BoundField();
                clm7.DataField = "NoPierna";
                gvGastos.Columns.Add(clm7);

                BoundField clm8 = new BoundField();
                clm8.DataField = "Proveedor";
                gvGastos.Columns.Add(clm8);

                BoundField clm9 = new BoundField();
                clm9.DataField = "Categoria";
                gvGastos.Columns.Add(clm9);

                BoundField clm10 = new BoundField();
                clm10.DataField = "Origen";
                gvGastos.Columns.Add(clm10);

                BoundField clm11 = new BoundField();
                clm11.DataField = "Destino";
                gvGastos.Columns.Add(clm11);

                gvGastos.DataSource = transacciones.gastosAe.OrderBy(x => x.Mes).ToList(); 
                gvGastos.DataBind();

                pnlGastos.Visible = true;
                pnlGastosViewRef.Visible = false;

                lblTotalTrasnRes.Text = totalRegistros.S();
                lblTotalRes.Text = string.Format("{0:###,###,###,0.##}", totalTransacciones) + " MXN";
                lblPromedioRes.Text = string.Format("{0:###,###,###,0.##}", promedio) + " MXN";
            }
            else if (tipoTransaccion == 3)
            {
                totalRegistros = transacciones.gastosProv.Count();
                contMeses = transacciones.gastosProv.GroupBy(r => r.Mes).Count();
                totalTransacciones = transacciones.gastosProv.Sum(x => x.Total);
                promedio = totalTransacciones / contMeses;

                BoundField clm = new BoundField();
                clm.DataField = "mes";
                gvGastos.Columns.Add(clm);

                BoundField clm3 = new BoundField();
                clm3.DataField = "Rubro";
                gvGastos.Columns.Add(clm3);

                BoundField clm4 = new BoundField();
                clm4.DataField = "Total";
                clm4.DataFormatString = "{0:c}";
                gvGastos.Columns.Add(clm4);

                BoundField clm6 = new BoundField();
                clm6.DataField = "Anio";
                gvGastos.Columns.Add(clm6);

                BoundField clm7 = new BoundField();
                clm7.DataField = "NoPierna";
                gvGastos.Columns.Add(clm7);

                BoundField clm8 = new BoundField();
                clm8.DataField = "Proveedor";
                gvGastos.Columns.Add(clm8);

                BoundField clm9 = new BoundField();
                clm9.DataField = "Categoria";
                gvGastos.Columns.Add(clm9);

                BoundField clm10 = new BoundField();
                clm10.DataField = "Origen";
                gvGastos.Columns.Add(clm10);

                BoundField clm11 = new BoundField();
                clm11.DataField = "Destino";
                gvGastos.Columns.Add(clm11);

                gvGastos.DataSource = transacciones.gastosProv.OrderBy(x => x.Mes).ToList(); 
                gvGastos.DataBind();

                pnlGastos.Visible = true;
                pnlGastosViewRef.Visible = false;

                lblTotalTrasnRes.Text = totalRegistros.S();
                lblTotalRes.Text = string.Format("{0:###,###,###,0.##}", totalTransacciones) + " MXN";
                lblPromedioRes.Text = string.Format("{0:###,###,###,0.##}", promedio) + " MXN";
            }
            else if (tipoTransaccion == 4)
            {
                totalRegistros = transacciones.vuelos.Count();
                contMeses = transacciones.vuelos.GroupBy(r => r.mes).Count();
                totalTiempoVuelo = SumatoriaTiempos(transacciones.vuelos.Select(x => x.tiempoVlo).ToList(), 0, 1);//transacciones.vuelos.Sum(x => x.Total);
                TiempoVueloProm = SumatoriaTiempos(transacciones.vuelos.Select(x => x.tiempoVlo).ToList(), totalRegistros, 2);

                BoundField clm = new BoundField();
                clm.DataField = "mes";
                gvGastos.Columns.Add(clm);

                BoundField clm2 = new BoundField();
                clm2.DataField = "anio";
                gvGastos.Columns.Add(clm2);

                BoundField clm3 = new BoundField();
                clm3.DataField = "origen";
                gvGastos.Columns.Add(clm3);

                BoundField clm4 = new BoundField();
                clm4.DataField = "destino";
                gvGastos.Columns.Add(clm4);

                BoundField clm5 = new BoundField();
                clm5.DataField = "origenVuelo";
                gvGastos.Columns.Add(clm5);

                BoundField clm6 = new BoundField();
                clm6.DataField = "destinoVuelo";
                gvGastos.Columns.Add(clm6);

                BoundField clm7 = new BoundField();
                clm7.DataField = "tiempoVlo";
                gvGastos.Columns.Add(clm7);

                BoundField clm8 = new BoundField();
                clm8.DataField = "categoria";
                gvGastos.Columns.Add(clm8);

                gvGastos.DataSource = transacciones.vuelos.OrderBy(x => x.mes).ToList(); 
                gvGastos.DataBind();

                pnlGastos.Visible = true;
                pnlGastosViewRef.Visible = false;

                lblTotalTrasnRes.Text = totalRegistros.S();
                lblTotalRes.Text = totalTiempoVuelo;
                lblPromedioRes.Text = TiempoVueloProm;
            }
            else if (tipoTransaccion == 5)
            {
                totalRegistros = transacciones.promedioPax.Count();
                contMeses = transacciones.promedioPax.GroupBy(r => r.mes).Count();
                totalTiempoVuelo = SumatoriaTiempos(transacciones.promedioPax.Select(x => x.tiempoVuelo).ToList(), 0, 1);//transacciones.vuelos.Sum(x => x.Total);
                TiempoVueloProm = Math.Round((transacciones.promedioPax.Sum(x => x.cantPax) / totalRegistros), 2).S();

                BoundField clm = new BoundField();
                clm.DataField = "mes";
                gvGastos.Columns.Add(clm);

                BoundField clm2 = new BoundField();
                clm2.DataField = "anio";
                gvGastos.Columns.Add(clm2);

                BoundField clm3 = new BoundField();
                clm3.DataField = "origen";
                gvGastos.Columns.Add(clm3);

                BoundField clm4 = new BoundField();
                clm4.DataField = "destino";
                gvGastos.Columns.Add(clm4);

                BoundField clm5 = new BoundField();
                clm5.DataField = "fechaOrigen";
                gvGastos.Columns.Add(clm5);

                BoundField clm6 = new BoundField();
                clm6.DataField = "fechaDestino";
                gvGastos.Columns.Add(clm6);

                BoundField clm7 = new BoundField();
                clm7.DataField = "tiempoVuelo";
                gvGastos.Columns.Add(clm7);

                BoundField clm8 = new BoundField();
                clm8.DataField = "cantPax";
                gvGastos.Columns.Add(clm8);

                BoundField clm9 = new BoundField();
                clm9.DataField = "cliente";
                gvGastos.Columns.Add(clm9);

                BoundField clm10 = new BoundField();
                clm10.DataField = "contrato";
                gvGastos.Columns.Add(clm10);

                gvGastos.DataSource = transacciones.promedioPax.OrderBy(x => x.mes).ToList(); 
                gvGastos.DataBind();

                pnlGastos.Visible = true;
                pnlGastosViewRef.Visible = false;

                lblTotalTrasnRes.Text = totalRegistros.S();
                lblTotalRes.Text = TiempoVueloProm;
                lblPromedioRes.Text = totalTiempoVuelo;
            }
            else if (tipoTransaccion == 6)
            {
                totalRegistros = transacciones.promedioCosto.Count();
                contMeses = transacciones.promedioCosto.GroupBy(r => r.mes).Count();
                totalTransacciones = transacciones.promedioCosto.Sum(x => x.importe);
                promedio = totalTransacciones / totalRegistros;

                BoundField clm = new BoundField();
                clm.DataField = "mes";
                gvGastos.Columns.Add(clm);

                BoundField clm2 = new BoundField();
                clm2.DataField = "anio";
                gvGastos.Columns.Add(clm2);

                BoundField clm3 = new BoundField();
                clm3.DataField = "rubro";
                gvGastos.Columns.Add(clm3);

                BoundField clm4 = new BoundField();
                clm4.DataField = "importe";
                clm4.DataFormatString = "{0:c}";
                gvGastos.Columns.Add(clm4);

                BoundField clm5 = new BoundField();
                clm5.DataField = "categoria";
                gvGastos.Columns.Add(clm5);

                BoundField clm6 = new BoundField();
                clm6.DataField = "tipodeGasto";
                gvGastos.Columns.Add(clm6);

                BoundField clm7 = new BoundField();
                clm7.DataField = "comentarios";
                gvGastos.Columns.Add(clm7);

                gvGastos.DataSource = transacciones.promedioCosto.OrderBy(x => x.mes).ToList(); 
                gvGastos.DataBind();

                pnlGastos.Visible = true;
                pnlGastosViewRef.Visible = false;

                lblTotalTrasnRes.Text = totalRegistros.S();
                lblTotalRes.Text = string.Format("{0:###,###,###,0.##}", totalTransacciones) + " MXN";
                lblPromedioRes.Text = string.Format("{0:###,###,###,0.##}", promedio) + " MXN";
            }
            else if (tipoTransaccion == 7)
            {
                totalRegistros = transacciones.horasVoladas.Count();
                contMeses = transacciones.horasVoladas.GroupBy(r => r.mes).Count();
                totalTiempoVuelo = SumatoriaTiempos(transacciones.horasVoladas.Select(x => x.tiempoVuelo).ToList(), 0, 1);//transacciones.vuelos.Sum(x => x.Total);
                TiempoVueloProm = SumatoriaTiempos(transacciones.horasVoladas.Select(x => x.tiempoVuelo).ToList(), totalRegistros, 2);

                BoundField clm = new BoundField();
                clm.DataField = "mes";
                gvGastos.Columns.Add(clm);

                BoundField clm2 = new BoundField();
                clm2.DataField = "anio";
                gvGastos.Columns.Add(clm2);

                BoundField clm3 = new BoundField();
                clm3.DataField = "origen";
                gvGastos.Columns.Add(clm3);

                BoundField clm4 = new BoundField();
                clm4.DataField = "destino";
                gvGastos.Columns.Add(clm4);

                BoundField clm5 = new BoundField();
                clm5.DataField = "fechaOrigen";
                gvGastos.Columns.Add(clm5);

                BoundField clm6 = new BoundField();
                clm6.DataField = "fechaDestino";
                gvGastos.Columns.Add(clm6);

                BoundField clm7 = new BoundField();
                clm7.DataField = "tiempoVuelo";
                gvGastos.Columns.Add(clm7);

                BoundField clm8 = new BoundField();
                clm8.DataField = "cantPax";
                gvGastos.Columns.Add(clm8);

                BoundField clm9 = new BoundField();
                clm9.DataField = "cliente";
                gvGastos.Columns.Add(clm9);

                BoundField clm10 = new BoundField();
                clm10.DataField = "contrato";
                gvGastos.Columns.Add(clm10);

                gvGastos.DataSource = transacciones.horasVoladas.OrderBy(x => x.mes).ToList(); 
                gvGastos.DataBind();

                pnlGastos.Visible = true;
                pnlGastosViewRef.Visible = false;

                lblTotalTrasnRes.Text = totalRegistros.S();
                lblTotalRes.Text = totalTiempoVuelo;
                lblPromedioRes.Text = TiempoVueloProm;
            }
            else if (tipoTransaccion == 8)
            {
                totalRegistros = transacciones.numeroVuelos.Count();
                contMeses = transacciones.numeroVuelos.GroupBy(r => r.mes).Count();
                totalTiempoVuelo = SumatoriaTiempos(transacciones.numeroVuelos.Select(x => x.tiempoVuelo).ToList(), 0, 1);//transacciones.vuelos.Sum(x => x.Total);
                TiempoVueloProm = SumatoriaTiempos(transacciones.numeroVuelos.Select(x => x.tiempoVuelo).ToList(), totalRegistros, 2);

                BoundField clm = new BoundField();
                clm.DataField = "mes";
                gvGastos.Columns.Add(clm);

                BoundField clm2 = new BoundField();
                clm2.DataField = "anio";
                gvGastos.Columns.Add(clm2);

                BoundField clm3 = new BoundField();
                clm3.DataField = "origen";
                gvGastos.Columns.Add(clm3);

                BoundField clm4 = new BoundField();
                clm4.DataField = "destino";
                gvGastos.Columns.Add(clm4);

                BoundField clm5 = new BoundField();
                clm5.DataField = "fechaOrigen";
                gvGastos.Columns.Add(clm5);

                BoundField clm6 = new BoundField();
                clm6.DataField = "fechaDestino";
                gvGastos.Columns.Add(clm6);

                BoundField clm7 = new BoundField();
                clm7.DataField = "tiempoVuelo";
                gvGastos.Columns.Add(clm7);

                BoundField clm8 = new BoundField();
                clm8.DataField = "cantPax";
                gvGastos.Columns.Add(clm8);

                BoundField clm9 = new BoundField();
                clm9.DataField = "cliente";
                gvGastos.Columns.Add(clm9);

                BoundField clm10 = new BoundField();
                clm10.DataField = "contrato";
                gvGastos.Columns.Add(clm10);

                gvGastos.DataSource = transacciones.numeroVuelos.OrderBy(x => x.mes).ToList(); 
                gvGastos.DataBind();

                pnlGastos.Visible = true;
                pnlGastosViewRef.Visible = false;

                lblTotalTrasnRes.Text = totalRegistros.S();
                lblTotalRes.Text = totalTiempoVuelo;
                lblPromedioRes.Text = TiempoVueloProm;
            }
            else if (tipoTransaccion == 9)
            {
                totalRegistros = transacciones.costosFijosVariable.Count();
                contMeses = transacciones.costosFijosVariable.GroupBy(r => r.mes).Count();
                totalTransacciones = transacciones.costosFijosVariable.Sum(x => x.totalImp);
                promedio = totalTransacciones / totalRegistros;

                BoundField clm = new BoundField();
                clm.DataField = "mes";
                gvGastos.Columns.Add(clm);

                BoundField clm2 = new BoundField();
                clm2.DataField = "anio";
                gvGastos.Columns.Add(clm2);

                BoundField clm3 = new BoundField();
                clm3.DataField = "rubro";
                gvGastos.Columns.Add(clm3);

                BoundField clm4 = new BoundField();
                clm4.DataField = "totalImp";
                clm4.DataFormatString = "{0:c}";
                gvGastos.Columns.Add(clm4);

                BoundField clm5 = new BoundField();
                clm5.DataField = "categoria";
                gvGastos.Columns.Add(clm5);

                BoundField clm6 = new BoundField();
                clm6.DataField = "idTipoGasto";
                gvGastos.Columns.Add(clm6);

                BoundField clm7 = new BoundField();
                clm7.DataField = "comentarios";
                gvGastos.Columns.Add(clm7);

                gvGastos.DataSource = transacciones.costosFijosVariable.OrderBy(x => x.mes).ToList(); 
                gvGastos.DataBind();

                pnlGastos.Visible = true;
                pnlGastosViewRef.Visible = false;

                lblTotalTrasnRes.Text = totalRegistros.S();
                lblTotalRes.Text = string.Format("{0:###,###,###,0.##}", totalTransacciones) + " MXN";
                lblPromedioRes.Text = string.Format("{0:###,###,###,0.##}", promedio) + " MXN";
            }
            else if (tipoTransaccion == 10)
            {
                totalRegistros = transacciones.gastosTotales.Count();
                contMeses = transacciones.gastosTotales.GroupBy(r => r.mes).Count();
                totalTransacciones = transacciones.gastosTotales.Sum(x => x.totalImp);
                promedio = totalTransacciones / totalRegistros;

                BoundField clm = new BoundField();
                clm.DataField = "mes";
                gvGastos.Columns.Add(clm);

                BoundField clm2 = new BoundField();
                clm2.DataField = "anio";
                gvGastos.Columns.Add(clm2);

                BoundField clm3 = new BoundField();
                clm3.DataField = "rubro";
                gvGastos.Columns.Add(clm3);

                BoundField clm4 = new BoundField();
                clm4.DataField = "totalImp";
                clm4.DataFormatString = "{0:c}";
                gvGastos.Columns.Add(clm4);

                BoundField clm5 = new BoundField();
                clm5.DataField = "categoria";
                gvGastos.Columns.Add(clm5);

                BoundField clm6 = new BoundField();
                clm6.DataField = "comentarios";
                gvGastos.Columns.Add(clm6);

                gvGastos.DataSource = transacciones.gastosTotales.OrderBy(x => x.mes).ToList(); 
                gvGastos.DataBind();

                pnlGastos.Visible = true;
                pnlGastosViewRef.Visible = false;

                lblTotalTrasnRes.Text = totalRegistros.S();
                lblTotalRes.Text = string.Format("{0:###,###,###,0.##}", totalTransacciones) + " MXN";
                lblPromedioRes.Text = string.Format("{0:###,###,###,0.##}", promedio) + " MXN";
            }
            else if (tipoTransaccion == 11)
            {
                totalRegistros = transacciones.costosHoraVuelo.Count();
                contMeses = transacciones.costosHoraVuelo.GroupBy(r => r.mes).Count();
                totalTransacciones = transacciones.costosHoraVuelo.Sum(x => x.totalImp);
                promedio = totalTransacciones / totalRegistros;

                BoundField clm = new BoundField();
                clm.DataField = "mes";
                gvGastos.Columns.Add(clm);

                BoundField clm2 = new BoundField();
                clm2.DataField = "anio";
                gvGastos.Columns.Add(clm2);

                BoundField clm3 = new BoundField();
                clm3.DataField = "rubro";
                gvGastos.Columns.Add(clm3);

                BoundField clm4 = new BoundField();
                clm4.DataField = "totalImp";
                clm4.DataFormatString = "{0:c}";
                gvGastos.Columns.Add(clm4);

                BoundField clm5 = new BoundField();
                clm5.DataField = "categoria";
                gvGastos.Columns.Add(clm5);

                BoundField clm6 = new BoundField();
                clm6.DataField = "comentarios";
                gvGastos.Columns.Add(clm6);

                gvGastos.DataSource = transacciones.costosHoraVuelo.OrderBy(x => x.mes).ToList(); 
                gvGastos.DataBind();

                pnlGastos.Visible = true;
                pnlGastosViewRef.Visible = false;

                lblTotalTrasnRes.Text = totalRegistros.S();
                lblTotalRes.Text = string.Format("{0:###,###,###,0.##}", totalTransacciones) + " MXN";
                lblPromedioRes.Text = string.Format("{0:###,###,###,0.##}", promedio) + " MXN";
            }
            else if (tipoTransaccion == 12)
            {

                totalRegistros = transacciones.costosFijosVariableHora.Count();
                contMeses = transacciones.costosFijosVariableHora.GroupBy(r => r.mes).Count();
                totalTransacciones = transacciones.costosFijosVariableHora.Sum(x => x.totalImp);
                promedio = totalTransacciones / totalRegistros;

                BoundField clm = new BoundField();
                clm.DataField = "mes";
                gvGastos.Columns.Add(clm);

                BoundField clm2 = new BoundField();
                clm2.DataField = "anio";
                gvGastos.Columns.Add(clm2);

                BoundField clm3 = new BoundField();
                clm3.DataField = "rubro";
                gvGastos.Columns.Add(clm3);

                BoundField clm4 = new BoundField();
                clm4.DataField = "totalImp";
                clm4.DataFormatString = "{0:c}";
                gvGastos.Columns.Add(clm4);

                BoundField clm5 = new BoundField();
                clm5.DataField = "categoria";
                gvGastos.Columns.Add(clm5);

                BoundField clm6 = new BoundField();
                clm6.DataField = "comentarios";
                gvGastos.Columns.Add(clm6);

                gvGastos.DataSource = transacciones.costosFijosVariableHora.OrderBy(x => x.mes).ToList(); 
                gvGastos.DataBind();

                pnlGastos.Visible = true;
                pnlGastosViewRef.Visible = false;

                lblTotalTrasnRes.Text = string.Format("{0:###,###,###,0.##}", totalTransacciones) + " MXN";
                lblTotalRes.Text = string.Format("{0:###,###,###,0.##}", Convert.ToDecimal(dataOpt.campo1)) + " MXN";
                lblPromedioRes.Text = dataOpt.campo2;
            }
            else if (tipoTransaccion == 13)
            {
                totalRegistros = transacciones.detalleEdoCuenta.Count();
                contMeses = 1;
                //totalTransacciones = transacciones.detalleEdoCuenta.Sum(x => x.importe.S().Db());
                //promedio = totalTransacciones / totalRegistros;

                BoundField clm = new BoundField();
                clm.DataField = "nombreMes";
                gvGastos.Columns.Add(clm);

                //BoundField clm2 = new BoundField();
                //clm2.DataField = "anio";
                //gvGastos.Columns.Add(clm2);

                BoundField clm3 = new BoundField();
                clm3.DataField = "tipoMoneda";
                gvGastos.Columns.Add(clm3);

                BoundField clm4 = new BoundField();
                clm4.DataField = "fecha";
                gvGastos.Columns.Add(clm4);

                BoundField clm5 = new BoundField();
                clm5.DataField = "noReferencia";
                gvGastos.Columns.Add(clm5);


                BoundField clm6 = new BoundField();
                clm6.DataField = "tipoGasto";
                gvGastos.Columns.Add(clm6);

                BoundField clm7 = new BoundField();
                clm7.DataField = "concepto";
                gvGastos.Columns.Add(clm7);

                BoundField clm8 = new BoundField();
                clm8.DataField = "detalle";
                gvGastos.Columns.Add(clm8);

                BoundField clm9 = new BoundField();
                clm9.DataField = "proveedor";
                gvGastos.Columns.Add(clm9);


                BoundField clm10 = new BoundField();
                clm10.DataField = "importe";
                clm10.DataFormatString = "{0:c}";
                gvGastos.Columns.Add(clm10);

                gvGastos.DataSource = transacciones.detalleEdoCuenta.Where(x => x.tipoMoneda == "MXN").OrderBy(x => x.mes).ToList(); 

                gvGastos.DataBind();

                pnlGastos.Visible = false;
                pnlGastosViewRef.Visible = true;
                gvGastosRef.DataSource = transacciones.detalleEdoCuenta.Where(x => x.tipoMoneda == "MXN").OrderBy(x => x.noReferencia).ToList();
                gvGastosRef.DataBind();
                ControlReferencias(gvGastosRef);

                lblTotalTrasnRes.Text = totalRegistros.S();

                lblTotal.Text = Properties.Resources.TabTran_TotalTranMXN;
                double totalTransaccionesMXN = transacciones.detalleEdoCuenta.Where(x => x.tipoMoneda == "MXN").Sum(x => x.importe.S().Db());
                lblTotalRes.Text = string.Format("{0:###,###,###,0.##}", totalTransaccionesMXN) + " MXN";

                lblPromedio.Text = Properties.Resources.TabTran_TotalTranUSD;
                totalTransacciones = transacciones.detalleEdoCuenta.Where(x => x.tipoMoneda == "USD").Sum(x => x.importe.S().Db());
                lblPromedioRes.Text = string.Format("{0:###,###,###,0.##}", totalTransacciones) + " USD";
            }
            else if (tipoTransaccion == 14)
            {

                totalRegistros = transacciones.detGastos.Count();
                contMeses = transacciones.detGastos.GroupBy(r => r.mes).Count();
                totalTransacciones = transacciones.detGastos.Sum(x => x.total);
                promedio = totalTransacciones / contMeses;

                BoundField clm = new BoundField();
                clm.DataField = "mes";
                gvGastos.Columns.Add(clm);

                BoundField clm2 = new BoundField();
                clm2.DataField = "anio";
                gvGastos.Columns.Add(clm2);

                BoundField clm3 = new BoundField();
                clm3.DataField = "rubro";
                gvGastos.Columns.Add(clm3);

                BoundField clm4 = new BoundField();
                clm4.DataField = "total";
                clm4.DataFormatString = "{0:c}";
                gvGastos.Columns.Add(clm4);

                BoundField clm5 = new BoundField();
                clm5.DataField = "categoria";
                gvGastos.Columns.Add(clm5);

                BoundField clm6 = new BoundField();
                clm6.DataField = "tipoGasto";
                gvGastos.Columns.Add(clm6);

                BoundField clm7 = new BoundField();
                clm7.DataField = "comentarios";
                gvGastos.Columns.Add(clm7);

                gvGastos.DataSource = transacciones.detGastos.OrderBy(x => x.mes).ToList(); 
                gvGastos.DataBind();

                pnlGastos.Visible = true;
                pnlGastosViewRef.Visible = false;

                lblTotalTrasnRes.Text = totalRegistros.S();
                lblTotalRes.Text = string.Format("{0:###,###,###,0.##}", totalTransacciones) + " " + (string)Session["tipoDet"]; 
                lblPromedioRes.Text = string.Format("{0:###,###,###,0.##}", promedio) + " " + (string)Session["tipoDet"];
            }
        }

        private void LlenarGVUSD(Transacciones transacciones, int tipoTransaccion)
        {
            var dataOpt = (camposOpcionales)Session["dataOpt"];
            gvGastosUSD.DataSource = null;

            if (tipoTransaccion == 13)
            {
                //pnlGastos.Visible = false;
                //pnlGastosViewRef.Visible = false;

                //DataTable dtGastosUSD = new DataTable();
                //dtGastosUSD = transacciones.detalleEdoCuenta.Where(x => x.tipoMoneda == "USD").OrderBy(x => x.noReferencia).ToList().ConvertListToDataTable();

                gvGastosUSD.DataSource = transacciones.detalleEdoCuenta.Where(x => x.tipoMoneda == "USD").OrderBy(x => x.noReferencia).ToList();
                gvGastosUSD.DataBind();

                ControlReferenciasUSD(gvGastosUSD);
            }
        }

        private string SumatoriaTiempos(List<string> items, int totalReg, int tipoSum)
        {
            var horas = 0;
            var min = 0;
            var sumatoria = "00:00";

            foreach (var item in items)
            {
                List<string> data = item.Split(':').ToList();
                var entHoras = Convert.ToInt32(data[0]);
                var entMin = Convert.ToInt32(data[1]);

                horas += entHoras > 0 ? entHoras : 0;
                min += entMin > 0 ? entMin : 0;
            }

            if(tipoSum == 1)
            {
                var min_Horas = min / 60;
                var rest_min = min - (min_Horas * 60);

                var HORAS = (horas + min_Horas).ToString();
                var MIN = (rest_min).S();

                sumatoria = HORAS + ":" + MIN;
            }
            else
            {
                var horas_Min = horas * 60;
                var totalMin = horas_Min + min;
                var prom = totalMin / totalReg;

                var min_Horas = prom / 60;
                var rest_min = prom - (min_Horas * 60);

                var HORAS = (min_Horas).ToString();
                var MIN = (rest_min).S();

                sumatoria = HORAS + ":" + MIN;
            }

            return sumatoria;
        }

        private void ArmarTransacciones()
        {
            var transaccion = Session["tipoTransaccion"];
            lblTransacciones.Text = (string)Session["title"] + " - MXN";
            lblTransacciones2.Text = (string)Session["title"] + " - USD";
            lblTitulo.Text = Properties.Resources.TabTransacciones;
            switch (transaccion)
            {
                case 4:
                    lblTotalTrasn.Text = Properties.Resources.TabTran_NoVuelos;
                    lblTotal.Text = Properties.Resources.TabTran_TiempoTotVuelo;
                    lblPromedio.Text = Properties.Resources.TabTran_PromedioVuelo;

                    iconTime.Visible = true;
                    iconMoney.Visible = false;
                    lblTotalSimbolo.Visible = false;
                    lblPromedioSimbolo.Visible = false;
                    break;
                case 5:
                    lblTotalTrasn.Text = Properties.Resources.TabTran_NoVuelos;
                    lblTotal.Text = Properties.Resources.TabTran_promedioPax;
                    lblPromedio.Text = Properties.Resources.TabTran_TiempoTotVuelo;

                    iconTime.Visible = true;
                    iconMoney.Visible = false;
                    lblTotalSimbolo.Visible = false;
                    lblPromedioSimbolo.Visible = false;
                    break;
                case 7:
                    lblTotalTrasn.Text = Properties.Resources.TabTran_NoVuelos;
                    lblTotal.Text = Properties.Resources.TabTran_TiempoTotVuelo;
                    lblPromedio.Text = Properties.Resources.TabTran_PromedioVuelo;

                    iconTime.Visible = true;
                    iconMoney.Visible = false;
                    lblTotalSimbolo.Visible = false;
                    lblPromedioSimbolo.Visible = false;
                    break;
                case 8:
                    lblTotalTrasn.Text = Properties.Resources.TabTran_NoVuelos;
                    lblTotal.Text = Properties.Resources.TabTran_TiempoTotVuelo;
                    lblPromedio.Text = Properties.Resources.TabTran_PromedioVuelo;

                    iconTime.Visible = true;
                    iconMoney.Visible = false;
                    lblTotalSimbolo.Visible = false;
                    lblPromedioSimbolo.Visible = false;
                    break;
                case 12:
                    lblTotalTrasn.Text = Properties.Resources.TabTran_MontoTotal;
                    lblTotal.Text = Properties.Resources.TabTran_CostoHV;
                    lblPromedio.Text = Properties.Resources.TabTran_TiempoTotVuelo;

                    iconTime.Visible = true;
                    iconMoney.Visible = false;
                    lblTotalSimbolo.Visible = false;
                    lblPromedioSimbolo.Visible = false;
                    break;
                default:
                    lblTotalTrasn.Text = Properties.Resources.TabTran_NoGastos;
                    lblTotal.Text = Properties.Resources.TabTran_MontoTotal;
                    lblPromedio.Text = Properties.Resources.TabTran_PromedioMens;

                    iconTime.Visible = false;
                    iconMoney.Visible = true;
                    lblTotalSimbolo.Visible = true;
                    lblPromedioSimbolo.Visible = true;
                    break;
            }

        }

        private string GenerateTitle(bool tipoT, int transaccion, string descripcion)
        {
            var title = "";

            switch (transaccion)
            {
                case 1: title = Utils.Idioma == "es-MX" ? "Costos por Categoria" : "Costs by Category";
                    break;
                case 2:
                    title = Utils.Idioma == "es-MX" ? "Gastos por Aeropuerto" : "Expenses by Airport";
                    break;
                case 3:
                    title = Utils.Idioma == "es-MX" ? "Gastos por Proveedor" : "Expenses by Vendor";
                    break;
                case 4:
                    title = Utils.Idioma == "es-MX" ? "Vuelos por Duracion" : "Flight Duration";
                    break;
                case 5:
                    title = Utils.Idioma == "es-MX" ? "Promedio Pasajeros" : "Average Passengers";
                    break;
                case 6:
                    title = Utils.Idioma == "es-MX" ? "Promedio Costo" : "Average Cost";
                    break;
                case 7:
                    title = Utils.Idioma == "es-MX" ? "Horas Voladas" : "Flight Hours";
                    break;
                case 8:
                    title = Utils.Idioma == "es-MX" ? "No. de Vuelos" : "No. of Flights";
                    break;
                case 9:
                    title = Utils.Idioma == "es-MX" ? "Costo Fijo y Variable" : "Fixed and Variable Cost";
                    break;
                case 10:
                    title = Utils.Idioma == "es-MX" ? "Gastos Totales" : "Total Expenses";
                    break;
                case 11:
                    title = Utils.Idioma == "es-MX" ? "Costo por Hora de Vuelo" : "Cost per Flight Hour";
                    break;
                case 12:
                    title = Utils.Idioma == "es-MX" ? "Costo Fijo y Variable por Hora" : "Fixed and Variable Cost Per Hour";
                    break;
                case 13:
                    title = Utils.Idioma == "es-MX" ? "Detalle de Estado de Cuenta" : "Account Statement Detail";
                    break;
                case 14:
                    title = Utils.Idioma == "es-MX" ? "Categorías a lo Largo del Tiempo" : "Categories Over Time";
                    break;
            }

            if (tipoT)
            {
                title += " - " + descripcion + " - " + Utils.MatriculaActual;
            }
            else
            {
                title += "_" + descripcion + "_" + Utils.MatriculaActual;
            } 

            return title;
        }

        [WebMethod]
        public static void ObtenerTransacciones(List<gasto> gastos, List<gastoAeropuerto> gastosAe, List<gastoProveedor> gastosProv, List<vuelo> vuelos, List<pasajero> paxs, List<costosProm> costos, List<hora> horasV, List<novuelo> novuelos, List<costofv> costosFV, List<gastot> gastosT, List<costohv> costoH, List<costofvh> costosFVH, List<detalleGastosTran> detGasto, int tipoTrans, string tipoDet, string descES, string descEN, int origen, camposOpcionales opt)
        {
            // tipo transaccion: 1 gastos
            // tipo transaccion: 2 gastosAe
            // tipo transaccion: 3 gastosProv
            // tipo transaccion: 4 vuelos
            // tipo transaccion: 5 promedio Paxs
            // tipo transaccion: 6 promedio costos
            // tipo transaccion: 7 horas voladas
            // tipo transaccion: 8 numero vuelos
            // tipo transaccion: 9 costos fijos variables
            // tipo transaccion: 10 gastos totales
            // tipo transaccion: 11 costos hora vuelo
            // tipo transaccion: 12 costos fijos variables por hora
            // tipo transaccion: 13 detalle estado de cuenta
            // tipo transaccion: 14 detalle gastos


            DateTimeFormatInfo month = null;
            CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
            TextInfo textInfo = cultureInfo.TextInfo;

            if (Utils.Idioma == "es-MX")
            {
                month = new CultureInfo("es-ES", false).DateTimeFormat;
            }
            else
            {
                month = new CultureInfo("en-US", false).DateTimeFormat;
            }
                

            if (tipoTrans == 1)
            {
                List<gvGastos> gv = new List<gvGastos>();
                foreach (var item in gastos)
                {
                    gvGastos g = new gvGastos();
                    if (tipoDet == "MXN" && item.totalUSD == 0 && item.totalMXN > 0)
                    {
                        g.idRubro = item.idRubro;
                        g.Rubro = Utils.Idioma == "es-MX" ? item.rubroESP : item.rubroENG;
                        g.Total = item.totalMXN;
                        g.Fecha = item.fecha;
                        g.Categoria = item.categoria;
                        g.tipodeGasto = item.tipodeGasto;
                        g.comentarios = item.comentarios;
                        g.mes = item.mes;

                        gv.Add(g);
                    }

                    if (tipoDet == "USD" && item.totalMXN == 0 && item.totalUSD > 0)
                    {
                        g.idRubro = item.idRubro;
                        g.Rubro = Utils.Idioma == "es-MX" ? item.rubroESP : item.rubroENG;
                        g.Total = item.totalUSD;
                        g.Fecha = item.fecha;
                        g.Categoria = item.categoria;
                        g.tipodeGasto = item.tipodeGasto;
                        g.comentarios = item.comentarios;
                        g.mes = item.mes;

                        gv.Add(g);
                    }

                }

                HttpContext.Current.Session["data"] = gv;
            }

            else if (tipoTrans == 2)
            {
                List<gvGastosAeropuerto> gva = new List<gvGastosAeropuerto>();
                foreach (var item in gastosAe)
                {
                    gvGastosAeropuerto g = new gvGastosAeropuerto();
                    g.IdRubro = item.idRubro.Value;
                    g.Rubro = Utils.Idioma == "es-MX" ? item.rubroESP : item.rubroENG;
                    g.Total = item.totalMXN;
                    g.Mes = textInfo.ToTitleCase(month.GetMonthName(item.mes.Value));
                    g.Anio = item.anio.HasValue ? item.anio.Value : 0;
                    g.NoPierna = item.noPierna.HasValue ? item.noPierna.Value : 0;
                    g.Proveedor = item.proveedor;
                    g.Categoria = item.categoria;
                    g.Origen = item.origen;
                    g.TipoMoneda = item.tipoMoneda;
                    g.Destino = item.destino;

                    gva.Add(g);
                }

                HttpContext.Current.Session["data"] = gva;
            }

            else if (tipoTrans == 3)
            {
                List<gvGastosProveedor> gvp = new List<gvGastosProveedor>();
                foreach (var item in gastosProv)
                {
                    gvGastosProveedor g = new gvGastosProveedor();
                    g.IdRubro = item.idRubro.Value;
                    g.Rubro = Utils.Idioma == "es-MX" ? item.rubroESP : item.rubroENG;
                    g.Total = item.totalMXN;
                    g.Mes = textInfo.ToTitleCase(month.GetMonthName(item.mes.Value));
                    g.Anio = item.anio.HasValue ? item.anio.Value : 0;
                    g.NoPierna = item.noPierna.HasValue ? item.noPierna.Value : 0;
                    g.Proveedor = item.proveedor;
                    g.Categoria = item.categoria;
                    g.Origen = item.origen;
                    g.TipoMoneda = item.tipoMoneda;
                    g.Destino = item.destino;

                    gvp.Add(g);
                }

                HttpContext.Current.Session["data"] = gvp;
            }

            else if (tipoTrans == 4)
            {
                List<vuelo> gvv = new List<vuelo>();
                foreach (var item in vuelos)
                {
                    vuelo g = new vuelo();
                    g.mes = textInfo.ToTitleCase(month.GetMonthName(Convert.ToInt32(item.mes)));
                    g.anio = item.anio;
                    g.origen = item.origen;
                    g.destino = item.destino;
                    g.origenVuelo = Convert.ToDateTime(item.origenVuelo).ToString("dd/MM/yyyy HH:mm");
                    g.destinoVuelo = Convert.ToDateTime(item.destinoVuelo).ToString("dd/MM/yyyy HH:mm");
                    g.tiempoVlo = item.tiempoVlo;
                    g.categoria = item.categoria;

                    gvv.Add(g);
                }

                HttpContext.Current.Session["data"] = gvv;
            }

            else if (tipoTrans == 5)
            {
                List<gvPromedioPax> gvpp = new List<gvPromedioPax>();
                foreach (var item in paxs)
                {
                    gvPromedioPax pp = new gvPromedioPax();
                    pp.mes = textInfo.ToTitleCase(month.GetMonthName(Convert.ToInt32(item.mes)));
                    pp.anio = item.anio.S();
                    pp.origen = item.origen;
                    pp.destino = item.destino;
                    pp.fechaOrigen = Convert.ToDateTime(item.origenVuelo).ToString("dd/MM/yyyy HH:mm");
                    pp.fechaDestino = Convert.ToDateTime(item.destinoVuelo).ToString("dd/MM/yyyy HH:mm");
                    pp.tiempoVuelo = item.tiempoVuelo;
                    pp.cantPax = item.cantPax;
                    pp.cliente = item.cliente;
                    pp.contrato = item.contrato;

                    gvpp.Add(pp);
                }

                HttpContext.Current.Session["data"] = gvpp;
            }

            else if (tipoTrans == 6)
            {
                List<gvPromedioCosto> gvpc = new List<gvPromedioCosto>();
                foreach (var item in costos)
                {
                    gvPromedioCosto pc = new gvPromedioCosto();
                    pc.mes = textInfo.ToTitleCase(month.GetMonthName(Convert.ToInt32(item.mes)));
                    pc.anio = item.anio.S();
                    pc.rubro = Utils.Idioma == "es-MX" ? item.rubroESP : item.rubroENG;
                    pc.importe = item.importe;
                    pc.categoria = Utils.Idioma == "es-MX" ? item.categoriaESP : item.categoriaENG;
                    pc.tipodeGasto = item.tipodeGasto;
                    pc.comentarios = item.comentarios;

                    gvpc.Add(pc);
                }

                HttpContext.Current.Session["data"] = gvpc;
            }

            else if (tipoTrans == 7)
            {
                List<gvhorasVoladas> gvhv = new List<gvhorasVoladas>();
                foreach (var item in horasV)
                {
                    gvhorasVoladas hv = new gvhorasVoladas();
                    hv.mes = textInfo.ToTitleCase(month.GetMonthName(Convert.ToInt32(item.mes)));
                    hv.anio = item.anio.S();
                    hv.origen = item.origen;
                    hv.destino = item.destino;
                    hv.fechaOrigen = Convert.ToDateTime(item.origenVuelo).ToString("dd/MM/yyyy HH:mm");
                    hv.fechaDestino = Convert.ToDateTime(item.destinoVuelo).ToString("dd/MM/yyyy HH:mm");
                    hv.tiempoVuelo = item.tiempoVuelo;
                    hv.cantPax = item.cantPax.S();
                    hv.cliente = item.cliente;
                    hv.contrato = item.contrato;

                    gvhv.Add(hv);
                }

                HttpContext.Current.Session["data"] = gvhv;
            }

            else if (tipoTrans == 8)
            {
                List<gvnoVuelos> gvnv = new List<gvnoVuelos>();
                foreach (var item in novuelos)
                {
                    gvnoVuelos nv = new gvnoVuelos();
                    nv.mes = textInfo.ToTitleCase(month.GetMonthName(Convert.ToInt32(item.mes)));
                    nv.anio = item.anio.S();
                    nv.origen = item.origen;
                    nv.destino = item.destino;
                    nv.fechaOrigen = Convert.ToDateTime(item.origenVuelo).ToString("dd/MM/yyyy HH:mm");
                    nv.fechaDestino = Convert.ToDateTime(item.destinoVuelo).ToString("dd/MM/yyyy HH:mm");
                    nv.tiempoVuelo = item.tiempoVuelo;
                    nv.cantPax = item.cantPax.S();
                    nv.cliente = item.cliente;
                    nv.contrato = item.contrato;

                    gvnv.Add(nv);
                }

                HttpContext.Current.Session["data"] = gvnv;
            }

            else if (tipoTrans == 9)
            {
                List<gvCostosFV> gvgfv = new List<gvCostosFV>();
                foreach (var item in costosFV)
                {
                    gvCostosFV gfv = new gvCostosFV();
                    gfv.rubro = Utils.Idioma == "es-MX" ? item.rubroESP : item.rubroENG;
                    gfv.totalImp = item.totalImp;
                    gfv.categoria = item.categoria;
                    gfv.comentarios = item.comentarios;
                    gfv.idTipoGasto = item.idTipoGasto.S();
                    gfv.mes = textInfo.ToTitleCase(month.GetMonthName(Convert.ToInt32(item.mes)));
                    gfv.anio = item.anio.S();

                    gvgfv.Add(gfv);
                }

                HttpContext.Current.Session["data"] = gvgfv;
            }

            else if (tipoTrans == 10)
            {
                List<gvGastosT> gvgt = new List<gvGastosT>();
                foreach (var item in gastosT)
                {
                    gvGastosT gt = new gvGastosT();
                    gt.rubro = Utils.Idioma == "es-MX" ? item.rubroESP : item.rubroENG;
                    gt.totalImp = item.totalImp;
                    gt.categoria = Utils.Idioma == "es-MX" ? item.categoriaESP : item.categoriaENG;
                    gt.comentarios = item.comentarios;
                    gt.mes = textInfo.ToTitleCase(month.GetMonthName(Convert.ToInt32(item.mes)));
                    gt.anio = item.anio.S();

                    gvgt.Add(gt);
                }

                HttpContext.Current.Session["data"] = gvgt;
            }

            else if (tipoTrans == 11)
            {
                List<gvCostosH> gvch = new List<gvCostosH>();
                foreach (var item in costoH)
                {
                    gvCostosH ch = new gvCostosH();
                    ch.rubro = Utils.Idioma == "es-MX" ? item.rubroENG : item.rubroENG;
                    ch.totalImp = item.totalImp;
                    ch.categoria = Utils.Idioma == "es-MX" ? item.categoriaESP : item.categoriaENG;
                    ch.comentarios = item.comentarios;
                    ch.mes = textInfo.ToTitleCase(month.GetMonthName(Convert.ToInt32(item.mes)));
                    ch.anio = item.anio.S();

                    gvch.Add(ch);
                }

                HttpContext.Current.Session["data"] = gvch;
            }

            else if (tipoTrans == 12)
            {
                List<gvCostosFVH> gvch = new List<gvCostosFVH>();
                foreach (var item in costosFVH)
                {
                    gvCostosFVH ch = new gvCostosFVH();
                    ch.rubro = Utils.Idioma == "es-MX" ? item.rubroESP : item.rubroENG;
                    ch.totalImp = item.totalImp;
                    ch.categoria = item.categoria;
                    ch.comentarios = item.comentarios;
                    ch.mes = textInfo.ToTitleCase(month.GetMonthName(Convert.ToInt32(item.mes)));
                    ch.anio = item.anio.S();

                    gvch.Add(ch);
                }

                HttpContext.Current.Session["data"] = gvch;
                HttpContext.Current.Session["dataOpt"] = opt;
            }

            else if (tipoTrans == 14)
            {
                List<gvDetGastos> gvdt = new List<gvDetGastos>();
                foreach (var item in detGasto)
                {
                    gvDetGastos dt = new gvDetGastos();
                    dt.rubro = Utils.Idioma == "es-MX" ? item.rubroESP : item.rubroENG;
                    dt.total = tipoDet == "MXN" ? item.totalMXN : item.totalUSD;
                    dt.categoria = item.categoria;
                    dt.comentarios = item.comentarios;
                    dt.tipoGasto = item.tipoGasto;
                    dt.mes = textInfo.ToTitleCase(month.GetMonthName(Convert.ToInt32(item.mes)));
                    dt.anio = item.anio.S();

                    gvdt.Add(dt);
                }

                HttpContext.Current.Session["data"] = gvdt;
                HttpContext.Current.Session["dataOpt"] = opt;
            }

            var descripcion = Utils.Idioma == "es-MX" ? CultureInfo.CurrentCulture.TextInfo.ToTitleCase(descES.ToLower()) : CultureInfo.CurrentCulture.TextInfo.ToTitleCase(descEN.ToLower());

            var det = tipoTrans == 8 ? Utils.MatriculaActual : tipoDet;

            HttpContext.Current.Session["origenData"] = origen;
            HttpContext.Current.Session["tipoDet"] = tipoDet;
            HttpContext.Current.Session["tipoTransaccion"] = tipoTrans;
            HttpContext.Current.Session["descripcion"] = descripcion;
            HttpContext.Current.Session["title"] = descripcion + " - " + det;
            HttpContext.Current.Session["titleFile"] = descripcion + "_" + det;
        }

        private void exportarExcel()
        {
            var nameFile = "Transacciones_" + (string)Session["titleFile"] + ".xls";
            Response.Clear();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", string.Format("attachment;filename={0}", nameFile));
            Response.ContentEncoding = Encoding.Default;
            //Response.Charset = "ISO-8859-1";
            Response.ContentType = "application/vnd.ms-excel";
            using (StringWriter sw = new StringWriter())
            {
                HtmlTextWriter hw = new HtmlTextWriter(sw);


                gvGastos.AllowPaging = false;
                var tipo = Convert.ToInt32(Session["tipoTransaccion"]);
                Transacciones transacciones = new Transacciones();
                if (tipo == 1)
                {
                    transacciones.gastos = (List<gvGastos>)Session["data"];
                    LlenarGV(transacciones, tipo);
                }

                else if (tipo == 2)
                {
                    transacciones.gastosAe = (List<gvGastosAeropuerto>)Session["data"];
                    LlenarGV(transacciones, tipo);
                }

                else if (tipo == 3)
                {
                    transacciones.gastosProv = (List<gvGastosProveedor>)Session["data"];
                    LlenarGV(transacciones, tipo);
                }

                else if (tipo == 4)
                {
                    transacciones.vuelos = (List<vuelo>)Session["data"];
                    LlenarGV(transacciones, tipo);
                }

                else if (tipo == 5)
                {
                    transacciones.promedioPax = (List<gvPromedioPax>)Session["data"];
                    LlenarGV(transacciones, tipo);
                }

                else if (tipo == 6)
                {
                    transacciones.promedioCosto = (List<gvPromedioCosto>)Session["data"];
                    LlenarGV(transacciones, tipo);
                }

                else if (tipo == 7)
                {
                    transacciones.horasVoladas = (List<gvhorasVoladas>)Session["data"];
                    LlenarGV(transacciones, tipo);
                }

                else if (tipo == 8)
                {
                    transacciones.numeroVuelos = (List<gvnoVuelos>)Session["data"];
                    LlenarGV(transacciones, tipo);
                }

                else if (tipo == 9)
                {
                    transacciones.costosFijosVariable = (List<gvCostosFV>)Session["data"];
                    LlenarGV(transacciones, tipo);
                }

                else if (tipo == 10)
                {
                    transacciones.gastosTotales = (List<gvGastosT>)Session["data"];
                    LlenarGV(transacciones, tipo);
                }

                else if (tipo == 11)
                {
                    transacciones.costosHoraVuelo = (List<gvCostosH>)Session["data"];
                    LlenarGV(transacciones, tipo);
                }

                else if (tipo == 12)
                {
                    transacciones.costosFijosVariableHora = (List<gvCostosFVH>)Session["data"];
                    LlenarGV(transacciones, tipo);
                }

                else if (tipo == 13)
                {
                    transacciones.detalleEdoCuenta = (List<detalleEdoCta>)Session["data"];
                    LlenarGV(transacciones, tipo);
                }

                else if (tipo == 14)
                {
                    transacciones.detGastos = (List<gvDetGastos>)Session["data"];
                    LlenarGV(transacciones, tipo);
                }


                gvGastos.HeaderRow.BackColor = Color.White;
                foreach (TableCell cell in gvGastos.HeaderRow.Cells)
                {
                    cell.BackColor = gvGastos.HeaderStyle.BackColor;
                }
                foreach (GridViewRow row in gvGastos.Rows)
                {
                    row.BackColor = Color.White;
                    foreach (TableCell cell in row.Cells)
                    {
                        if (row.RowIndex % 2 == 0)
                        {
                            cell.BackColor = gvGastos.AlternatingRowStyle.BackColor;
                        }
                        else
                        {
                            cell.BackColor = gvGastos.RowStyle.BackColor;
                        }
                        cell.CssClass = "textmode";
                    }
                }

                gvGastos.RenderControl(hw);

                string style = @"<style> .textmode { } </style>";
                Response.Write(sw.ToString());
                //Response.Output.Write();
                //Response.Flush();
                Response.End();
            }
        }

        private void exportarPDF()
        {
            var nameFile = "Transacciones_" + (string)Session["titleFile"] + ".pdf";
            Response.Clear();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", string.Format("attachment;filename={0}", nameFile));
            Response.ContentEncoding = Encoding.Default;
            //Response.Charset = "ISO-8859-1";
            Response.ContentType = "application/octet-stream";
            Response.Cache.SetCacheability(HttpCacheability.NoCache);

            gvGastos.AllowPaging = false;
            var tipo = Convert.ToInt32(Session["tipoTransaccion"]);
            Transacciones transacciones = new Transacciones();
            if (tipo == 1)
            {
                transacciones.gastos = (List<gvGastos>)Session["data"];
                LlenarGV(transacciones, tipo);
            }

            else if (tipo == 2)
            {
                transacciones.gastosAe = (List<gvGastosAeropuerto>)Session["data"];
                LlenarGV(transacciones, tipo);
            }

            else if (tipo == 3)
            {
                transacciones.gastosProv = (List<gvGastosProveedor>)Session["data"];
                LlenarGV(transacciones, tipo);
            }

            else if (tipo == 4)
            {
                transacciones.vuelos = (List<vuelo>)Session["data"];
                LlenarGV(transacciones, tipo);
            }

            else if (tipo == 5)
            {
                transacciones.promedioPax = (List<gvPromedioPax>)Session["data"];
                LlenarGV(transacciones, tipo);
            }

            else if (tipo == 6)
            {
                transacciones.promedioCosto = (List<gvPromedioCosto>)Session["data"];
                LlenarGV(transacciones, tipo);
            }

            else if (tipo == 7)
            {
                transacciones.horasVoladas = (List<gvhorasVoladas>)Session["data"];
                LlenarGV(transacciones, tipo);
            }

            else if (tipo == 8)
            {
                transacciones.numeroVuelos = (List<gvnoVuelos>)Session["data"];
                LlenarGV(transacciones, tipo);
            }

            else if (tipo == 9)
            {
                transacciones.costosFijosVariable = (List<gvCostosFV>)Session["data"];
                LlenarGV(transacciones, tipo);
            }

            else if (tipo == 10)
            {
                transacciones.gastosTotales = (List<gvGastosT>)Session["data"];
                LlenarGV(transacciones, tipo);
            }

            else if (tipo == 11)
            {
                transacciones.costosHoraVuelo = (List<gvCostosH>)Session["data"];
                LlenarGV(transacciones, tipo);
            }

            else if (tipo == 12)
            {
                transacciones.costosFijosVariableHora = (List<gvCostosFVH>)Session["data"];
                LlenarGV(transacciones, tipo);
            }

            else if (tipo == 13)
            {
                transacciones.detalleEdoCuenta = (List<detalleEdoCta>)Session["data"];
                LlenarGV(transacciones, tipo);
            }

            else if (tipo == 14)
            {
                transacciones.detGastos = (List<gvDetGastos>)Session["data"];
                LlenarGV(transacciones, tipo);
            }

            using (StringWriter sw = new StringWriter())
            {
                HtmlTextWriter hw = new HtmlTextWriter(sw);

                gvGastos.HeaderRow.BackColor = Color.White;
                foreach (TableCell cell in gvGastos.HeaderRow.Cells)
                {
                    cell.BackColor = gvGastos.HeaderStyle.BackColor;
                }
                foreach (GridViewRow row in gvGastos.Rows)
                {
                    row.BackColor = Color.White;
                    foreach (TableCell cell in row.Cells)
                    {
                        if (row.RowIndex % 2 == 0)
                        {
                            cell.BackColor = gvGastos.AlternatingRowStyle.BackColor;
                        }
                        else
                        {
                            cell.BackColor = gvGastos.RowStyle.BackColor;
                        }
                        cell.CssClass = "textmode";
                    }
                }

                gvGastos.RenderControl(hw);

                StringReader sr = new StringReader(sw.ToString());
                Document pdfDoc = new Document(PageSize.A4.Rotate(), 10f, 10f, 100f, 0f);
                HTMLWorker htmlparser = new HTMLWorker(pdfDoc);
                PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
                pdfDoc.Open();
                htmlparser.Parse(sr);
                pdfDoc.Close();
                Response.Write(pdfDoc);
                Response.End();
            }
        }

        protected void btnExcelUSD_Click(object sender, EventArgs e)
        {
            try
            {
                exportarExcelUSD();
            }
            catch (Exception ex)
            {
                string strError = ex.Message;
            }
        }

        private void exportarExcelUSD()
        {
            var nameFile = "Transacciones_" + (string)Session["titleFile"] + "_USD.xls";
            Response.Clear();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", string.Format("attachment;filename={0}", nameFile));
            Response.ContentEncoding = Encoding.Default;
            //Response.Charset = "ISO-8859-1";
            Response.ContentType = "application/vnd.ms-excel";

            using (StringWriter sw = new StringWriter())
            {
                HtmlTextWriter hw = new HtmlTextWriter(sw);
                gvGastosUSD.AllowPaging = false;
                var tipo = Convert.ToInt32(Session["tipoTransaccion"]);
                Transacciones transacciones = new Transacciones();

                if (tipo == 13)
                {
                    transacciones.detalleEdoCuenta = (List<detalleEdoCta>)Session["data"];
                    LlenarGVUSD(transacciones, tipo);                    
                }

                //string style = @"<style> .textmode { } </style>";
                gvGastosUSD.HeaderRow.BackColor = Color.White;
                foreach (TableCell cell in gvGastosUSD.HeaderRow.Cells)
                {
                    cell.BackColor = gvGastosUSD.HeaderStyle.BackColor;
                }
                foreach (GridViewRow row in gvGastosUSD.Rows)
                {
                    row.BackColor = Color.White;
                }

                gvGastosUSD.RenderControl(hw);
                Response.Write(sw.ToString());
                //Response.Output.Write();
                Response.Flush();
                Response.End();
            }    
        }

        #endregion

        protected void gvGastos_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            
        }

        private string ArmaRutaComprobante(string sReferencia, string[] sPeriodo)
        {
            try
            {
                string sRuta = string.Empty;
                string sMatricula = Utils.MatriculaActual;
                int iAnio = sPeriodo[1].I();
                string sMes = sPeriodo[0].S();
                int iMes = ObtenerNumeroMes(sMes);

                DataTable dt = new DBTransacciones().DBGetDetalleReferencia(sReferencia, sMatricula, iAnio.S(), iMes.S());
                if (dt.Rows.Count > 0)
                {
                    int iMesRef = dt.Rows[0]["Mes"].S().I();
                    int iAnioRef = dt.Rows[0]["Anio"].S().I();
                    string sMatriculaRef = dt.Rows[0]["Matricula"].S();
                    string sMoneda = dt.Rows[0]["TipoMoneda"].S();

                    sRuta = System.Configuration.ConfigurationManager.AppSettings["PATH_FILES_S"].S();
                    sRuta = sRuta.S().Replace("\\", "\\\\");
                    sRuta = sRuta.Replace("[anio]", iAnioRef.S());
                    sRuta = sRuta.Replace("[matricula]", sMatriculaRef);
                    sRuta = sRuta.Replace("[mes]", ObtieneNombreMes(iMesRef));
                    string sMon = sMoneda == "MXN" ? "MN" : "USD";
                    sRuta = sRuta.Replace("[moneda]", sMon);
                }

                return sRuta;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private string ObtieneNombreMes(int iMes)
        {
            string sMes = string.Empty;
            switch (iMes)
            {
                case 1:
                    sMes = "01 ENERO";
                    break;
                case 2:
                    sMes = "02 FEBRERO";
                    break;
                case 3:
                    sMes = "03 MARZO";
                    break;
                case 4:
                    sMes = "04 ABRIL";
                    break;
                case 5:
                    sMes = "05 MAYO";
                    break;
                case 6:
                    sMes = "06 JUNIO";
                    break;
                case 7:
                    sMes = "07 JULIO";
                    break;
                case 8:
                    sMes = "08 AGOSTO";
                    break;
                case 9:
                    sMes = "09 SEPTIEMBRE";
                    break;
                case 10:
                    sMes = "10 OCTUBRE";
                    break;
                case 11:
                    sMes = "11 NOVIEMBRE";
                    break;
                case 12:
                    sMes = "12 DICIEMBRE";
                    break;
            }

            return sMes;
        }

        public int ObtenerNumeroMes(string sMes)
        {
            try
            {
                int iMes = 0;
                switch (sMes)
                {
                    case "Enero":
                    case "January":
                        iMes = 1;
                        break;
                    case "Febrero":
                    case "February":
                        iMes = 2;
                        break;
                    case "Marzo":
                    case "March":
                        iMes = 3;
                        break;
                    case "Abril":
                    case "April":
                        iMes = 4;
                        break;
                    case "Mayo":
                    case "May":
                        iMes = 5;
                        break;
                    case "Junio":
                    case "June":
                        iMes = 6;
                        break;
                    case "Julio":
                    case "July":
                        iMes = 7;
                        break;
                    case "Agosto":
                    case "August":
                        iMes = 8;
                        break;
                    case "Septiembre":
                    case "September":
                        iMes = 9;
                        break;
                    case "Octubre":
                    case "October":
                        iMes = 10;
                        break;
                    case "Noviembre":
                    case "November":
                        iMes = 11;
                        break;
                    case "Diciembre":
                    case "December":
                        iMes = 12;
                        break;
                    default:
                        break;
                }
                return iMes;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public void ControlReferencias(GridView gv)
        {
            try
            {
                for (int i = 0; i < gv.Rows.Count; i++)
                {
                    Label lblReferencia = (Label)gv.Rows[i].FindControl("lblReferenciaPesos");
                    ImageButton imbReferencia = (ImageButton)gv.Rows[i].FindControl("imbReferenciaPesos");
                    string sMatricula = Utils.MatriculaActual;

                    DataSet ds = new DBTransacciones().DBGetComprobanteXRef(lblReferencia.Text, sMatricula);

                    if (ds != null)
                    {
                        if (ds.Tables.Count > 0)
                        {
                            if (ds.Tables[0].Rows.Count > 0)
                            {
                                if (imbReferencia != null)
                                {
                                    if (ds.Tables[0].Rows[0]["Comprobante"].S().I() == 1)
                                        imbReferencia.Visible = true;
                                    else
                                        imbReferencia.Visible = false;
                                }
                            }
                            else
                                imbReferencia.Visible = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void ControlReferenciasUSD(GridView gv)
        {
            try
            {
                for (int i = 0; i < gv.Rows.Count; i++)
                {
                    Label lblReferencia = (Label)gv.Rows[i].FindControl("lblReferenciaUSD");
                    ImageButton imbReferencia = (ImageButton)gv.Rows[i].FindControl("imbReferenciaUSD");
                    string sMatricula = Utils.MatriculaActual;

                    DataSet ds = new DBTransacciones().DBGetComprobanteXRef(lblReferencia.Text, sMatricula);

                    if (ds != null)
                    {
                        if (ds.Tables.Count > 0)
                        {
                            if (ds.Tables[1].Rows.Count > 0)
                            {
                                if (imbReferencia != null)
                                {
                                    if (ds.Tables[1].Rows[0]["Comprobante"].S().I() == 1)
                                        imbReferencia.Visible = true;
                                    else
                                        imbReferencia.Visible = false;
                                }
                            }
                            else
                                imbReferencia.Visible = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

    }
}
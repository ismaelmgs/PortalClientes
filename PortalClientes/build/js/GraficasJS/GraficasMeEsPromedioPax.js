$(document).ready(function () {
    //const url = "http://192.168.1.250/PortalClientes/Views/frmMetricasEstadisticas.aspx/GetPromedioPasajeros"; // API URL
    const urlPP = getUrlPP(); // API URL

    let objPP = JSON.stringify({
        meses: $("#ContentPlaceHolder1_DDFiltroMesesPP").val(),
    });

    ajax_dataPP(objPP, urlPP, function (dataPP) {
        chartsPP(dataPP , "PieChart"); // Pie Charts
    });

    window.onresize = function () {
        ajax_dataPP(objPP, urlPP, function (dataPP) {
            chartsPP(dataPP, "PieChart"); // Pie Charts
        });
    };
});

function getUrlPP() {
    let value = window.location + "/GetPromedioPasajeros";
    return value;
}

$('#ContentPlaceHolder1_DDFiltroMesesPP').change(function (event) {
    event.preventDefault();
    lPanel.Show();
    ActualizarGraficaPP();
});

function ActualizarGraficaPP() {
    const urlPP = getUrlPP(); // API URL
    let objPP = JSON.stringify({
        meses: $("#ContentPlaceHolder1_DDFiltroMesesPP").val(),
    });

    ajax_dataPP(objPP, urlPP, function (dataPP) {
        chartsPP(dataPP, "PieChart"); // Pie Charts
    });
}

function ajax_dataPP(objPP, urlPP, success) {
    $.ajax({
        data: objPP,
        contentType: "Application/json; charset=utf-8",
        responseType: "json",
        method: 'POST',
        url: urlPP,
        dataType: "json",
        beforeSend: function (response) { },
        success: function (response) {
            success.call(this, response.d);
        },
        error: function (err) {
            console.log("Error In Connecting", err);
        }
    });
}

function chartsPP(dataPP, ChartType) {
    var c = ChartType;
    var jsonDataPP = dataPP;

    if (jsonDataPP.length > 0) {
        google.charts.load("current", { packages: ["corechart"] });
        google.charts.setOnLoadCallback(drawVisualizationPP)
    }else{
        let mensaje='';
        let leng = document.getElementById('txtLang').value
        if (leng == "es-MX") {
            mensaje="No Hay Datos Disponibles";
        }else{
            mensaje="No data available"
        }
        
        document.getElementById('piechart_3d_15').innerHTML = `<div class="alert alert-info mt-5 text-center" role="alert">${mensaje}</div>`;
        lPanel.Hide();
    }
    
    function generarUrlPP(obtiene) {
        var url = "";

        if (obtiene) {
            if (window.location.host.includes("localhost")) {
                url = "/Views/frmTransacciones.aspx/ObtenerTransacciones";
            } else {
                url = "/PortalClientes/Views/frmTransacciones.aspx/ObtenerTransacciones";
            }
        }
        else {
            if (window.location.host.includes("localhost")) {
                url = "/Views/frmTransacciones.aspx";
            } else {
                url = "/PortalClientes/Views/frmTransacciones.aspx";
            }
        }

        return url;
    }

    function drawVisualizationPP() {

        var screenWidth = screen.width;

        var dataPP_ = new google.visualization.DataTable();
        dataPP_.addColumn('string', 'Meses');
        dataPP_.addColumn('number', jsonDataPP[0].idioma == "es-MX" ? "Promedio" : "Average");
        dataPP_.addColumn({ type: 'string', role: 'tooltip' });

        jsonDataPP.forEach((item, index) => {
            if (jsonDataPP[0].idioma == "es-MX") {
                dataPP_.addRows([[item.nombreESP, item.promPax, `Promedio en ${item.nombreESP} - ${item.promPax} pasajeros`,]]);
            } else {
                dataPP_.addRows([[item.nombreENG, item.promPax, `Average in ${item.nombreENG} - ${item.promPax} passengers`,]]);
            }
        });

        const colorsList = ['#3276ae','#6aabc0','#cf575e','#eb924f','#f6c543','#d578a9','#9889d1','#89d193','#d1b089','#e48fea','#f4d583','#fea6c0','#94e6f2','#89c893','#ffe1a1']

        var optionsPP = {
            title: jsonDataPP[0].idioma == "es-MX" ? "Promedio Pasajeros" : "Average Passengers",
            bar: {
                groupWidth: "60%",
            },
            fontSize: 12,
            chartArea: {
                left: screenWidth > 500 ? 30 : 10,
                top: 30,
                width: '100%',
                height: '75%'
            },
            animation: {
                duration: 3000,
                easing: 'out',
                startup: true
            },
            legend: {
                position: 'bottom',
                alignment: 'center',
            },
            colors: colorsList.sort(function () { return 0.5 - Math.random() }),
        };

        var chartPP = new google.visualization.ColumnChart(document.getElementById('piechart_3d_15'));
        chartPP.draw(dataPP_, optionsPP);

        lPanel.Hide();

        google.visualization.events.addListener(chartPP, 'select', function () {
            lPanel.Show();
            var selection = chartPP.getSelection();
            if (selection.length) {
                var row = selection[0].row;

                let array = jsonDataPP[row];
                const paxs = array.pax

                let opt = {
                    campo1: null,
                    campo2: null,
                }//campos opcionales en graficas

                let gastos = []
                let vuelos = []
                let gastosAe = []
                let gastosProv = []
                let costos = []
                let horasV = []
                let novuelos = []
                let gastosT = []
                let costoH = []
                let costosFV = []
                let costosFVH = []
                let detGasto = []

                let obj = JSON.stringify({
                    vuelos,
                    gastos,
                    gastosAe,
                    gastosProv,
                    costos,
                    paxs,
                    horasV,
                    novuelos,
                    costosFV,
                    gastosT,
                    costoH,
                    costosFVH,
                    detGasto,
                    tipoTrans: 5,
                    tipoDet: "MXN",
                    descES: array.nombreESP,
                    descEN: array.nombreENG,
                    origen: 2,
                    opt,
                });

                $.ajax({
                    data: obj,
                    contentType: "Application/json; charset=utf-8",
                    responseType: "json",
                    method: 'POST',
                    url: generarUrlPP(true),
                    dataType: "json",
                    success: function (response) {
                        window.location.pathname = generarUrlPP(false);
                    },
                    error: function (err) {
                        console.log("Error In Connecting", err);
                    }
                });
            }
        });
    }
}



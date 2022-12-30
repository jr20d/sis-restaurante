$(document).ready(function () {
    function cargarDatos() {
        $.ajax({
            method: "POST",
            url: "/factura/facturas",
            cache: false,
            dataType: "JSON",
            success: function (rst) {
                console.log(rst);
                var tabla = $("#tablaFactura").DataTable();
                tabla.destroy();

                var filas = document.getElementById("cTablaFacturas");

                filas.innerHTML = "";

                var indice = 0;
                rst.map((f) => {
                    filas.innerHTML += '<tr><td>' + f.Correlativo + '</td><td>' + f.Fecha + '</td><td>' + f.TipoFactura + '</td><td><button class="btn btn-info" type="button" id="factura-' + indice + '">Ver</button></td></tr>';
                    indice++;
                });

                tabla = $("#tablaFactura").DataTable();

                for (var i = 0; i < rst.length; i++) {
                    VerFactura(document.getElementById("factura-" + i), rst[i]);
                }
            }
        });
    }

    //Evento ver factura
    function VerFactura(boton, data) {
        boton.addEventListener("click", function (e) {
            $("#verFactura").modal("show");
            var factura = document.getElementById("factura");
            factura.setAttribute("src", "/Recursos/Facturas/Factura-" + data.ID + ".pdf");
        });
    }

    cargarDatos();
});
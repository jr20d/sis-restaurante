$(document).ready(function () {
    var datos;
    function cargarDatos(registros) {
        datos = registros;
        var tabla = $("#tablaSucursal").DataTable();
        tabla.destroy();
        var filas = document.getElementById("cTablaSucursal");
        filas.innerHTML = "";
        var indice = 0;
        var noEliminados = datos.filter((item) => item.Eliminar === 0);
        noEliminados.map((item) => {
            filas.innerHTML += ("<tr role='row'><td>" + item.Nombre + "</td><td>" + item.Telefono + "</td><td><buttom id='editSucursal-" + indice + "' class='btn btn-primary' data-toggle='modal' data-target='.editar-sucursal'>EDITAR</buttom><buttom id='elimSucursal-" + indice + "' class='btn btn-danger ml-1'>ELIMINAR</buttom></td></tr>");
            indice++;
            return indice;
        });
        
        for (var i = 0; i < noEliminados.length; i++) {
            CargarEditModal(document.getElementById("editSucursal-" + i), noEliminados[i]);
            QuitarSucursal(document.getElementById("elimSucursal-" + i), noEliminados[i]);
        }

        tabla = $("#tablaSucursal").DataTable();
    }

    function recuperarDatos() {
        $.ajax({
            method: "GET",
            url: "/datos/mostrarsucursales",
            Cache: false,
            processData: false,
            dataType: "JSON",
            success: function (resultado) {
                cargarDatos(resultado);
            }
        });
    }

    function QuitarSucursal(boton, datosBorrar) {
        boton.addEventListener("click", function (e) {
            AlertaBorrar(datosBorrar.ID, "/sucursal/eliminar", function (resultado) {
                if (resultado === true) {
                    MsgAgregado("La sucursal se ha borrado con éxito");
                    recuperarDatos();
                }
                else {
                    MsgNoAgregado("Error al realizar la acción de borrado");
                }
            });
        });
    }

    var formulario = document.getElementById("formEditarSucursal");

    function CargarEditModal(botonEditar, datosEdit) {
        botonEditar.addEventListener("click", function (e) {
            formulario.innerHTML = ('<div class="modal-header"><h3 class="modal-title" id="myLargeModalLabel">DATOS SUCURSAL</h3><button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">×</span></button></div><div class="grupoControles"><div class="controles"><label>Nombre sucursal:</label><input type="text" class="usuario control" value="' + datosEdit.Nombre + '" readonly /></div><div class="controles"><input type="text" name="telefono" class="textbox telefono" id="telefonoEdit" value="' + datosEdit.Telefono + '"><div class="texto noplaceholder">Telefono sucursal</div></div></div><div class="grupoControles"><div class="controles"><div class="textoarea noplaceholderareas">Dirección sucursal</div><textarea class="area" name="direccion" id="dirSucursal" readonly>' + datosEdit.Direccion + '</textarea></div></div><div class="grupoControles"><button id="guardarCambiosSucursal" class="btnEnviar" type="button">Guardar</button></div><div id="alertasEdit"></div>');
            EventosControles();
            document.getElementById("guardarCambiosSucursal").addEventListener("click", function (e) {
                var telEditControl = document.getElementById("telefonoEdit");
                var contenedorAlertas = document.getElementById("alertasEdit");
                contenedorAlertas.innerHTML = "";
                if (TieneContenido(telEditControl.value.trim()) && telEditControl.value.trim().length === 9) {
                    var buscarTel = datos.find((dato) => (dato.Telefono === telEditControl.value.trim() && dato.Telefono !== datosEdit.Telefono));
                    if (buscarTel === undefined) {
                        var DatosDTO = { ID: datosEdit.ID, Telefono: telEditControl.value.trim() }
                        $.ajax({
                            method: "POST",
                            url: "/sucursal/editar",
                            data: DatosDTO,
                            Cache: false,
                            success: function (Resultado) {
                                if (Resultado === "OK") {
                                    MsgAgregado("La sucursal se actualizó con éxito");
                                    OcultarVentana(".editar-sucursal");
                                    recuperarDatos();
                                }
                                else {
                                    MsgNoAgregado(Resultado);
                                }
                            }
                        });
                    }
                    else {
                        MensajeError(contenedorAlertas, "El telefono que intenta ingresar ya ha sido asignado a otra sucursal");
                    }
                }
                else {
                    MensajeError(contenedorAlertas, "Error en el número de telefono");
                }
            });

        });
    }

    recuperarDatos();

    var nombreControl = document.getElementById("nombre");
    var telefonoControl = document.getElementById("telefono");
    var dirControl = document.getElementById("dirSucursal");
    var mesasControl = document.getElementById("cantMesas");

    LimitarTexto(nombreControl, 50, true);
    SoloEnteros(mesasControl);

    var btnGuardar = document.getElementById("guardarSucursal");
    btnGuardar.addEventListener("click", function (e) {
        var nombre, telefono, direccion, mesas;
        var contenedor = document.getElementById("alertas");
        contenedor.innerHTML = "";
        if (TieneContenido(nombreControl.value.trim())) {
            if (datos.length > 0) {
                var buscar = datos.find((sucursal) => sucursal.Nombre.toUpperCase() === nombreControl.value.trim().toUpperCase());
                nombre = (buscar === undefined) ? nombreControl.value.trim().toUpperCase() : undefined;
                if (nombre === undefined) {
                    MensajeError(contenedor, "Ya existe un registro con este nombre");
                }
            }
            else {
                nombre = nombreControl.value.trim().toUpperCase();
            }
        }
        else {
            MensajeError(contenedor, "Debe ingresar el nombre de la sucursal");
        }

        if (TieneContenido(telefonoControl.value.trim()) && telefonoControl.value.trim().length === 9) {
            var buscarTel = datos.find((dato) => dato.Telefono === telefonoControl.value.trim());
            telefono = (buscarTel === undefined) ? telefonoControl.value.trim() : undefined;
            if (telefono === undefined) {
                MensajeError(contenedor, "El número de telefono ya ha sido ingresado");
            }
        }
        else {
            MensajeError(contenedor, "Número de telefono incorrecto");
        }

        if (TieneContenido(dirControl.value.trim())) {
            direccion = dirControl.value.trim();
        }
        else {
            MensajeError(contenedor, "Debe ingresar la dirección de la sucursal");
        }

        if (TieneContenido(mesasControl.value.trim())) {
            if (parseFloat(mesasControl.value.trim()) > 0) {
                mesas = parseFloat(mesasControl.value.trim());
            }
            else {
                MensajeError(contenedor, "La cantidad de mesas no puede ser de cero");
            }
        }
        else {
            MensajeError(contenedor, "Debe ingresar la cantidad de mesas para la sucursal");
        }

        if (nombre !== undefined && telefono !== undefined && direccion !== undefined && mesas !== undefined) {
            var SucursalData = { Nombre: nombre, Telefono: telefono, Direccion: direccion, Eliminar: 0 }
            DatosDTO = {SucursalDTO: SucursalData, CantMesas: mesas}

            $.ajax({
                method: "POST",
                url: "/sucursal/agregar",
                data: DatosDTO,
                Cache: false,
                success: function (Resultado) {
                    if (Resultado === "OK") {
                        MsgAgregado("La sucursal ha sido agregada con éxito");
                        LimpiarControl(dirControl);
                        LimpiarTextBox(".textbox", btnGuardar);
                        OcultarVentana(".agregar-sucursal");
                        recuperarDatos();
                    }
                    else {
                        MsgNoAgregado(Resultado);
                    }
                }
            });
        }
    });
});
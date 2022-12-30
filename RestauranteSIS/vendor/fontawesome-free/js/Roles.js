$(document).ready(function () {
    var datos;
    var nombre = document.getElementById("nombre");
    var salario = document.getElementById("salario");
    var agregar = document.getElementById("guardarRol");
    var alertas = document.getElementById("alertas");

    function cargarDatos(registros) {
        datos = registros;
        var tabla = $("#tablaRoles").DataTable();
        tabla.destroy();
        var filas = document.getElementById("cTablaRoles");
        filas.innerHTML = "";
        var indice = 0;
        datos.map((item) => {
            filas.innerHTML += ("<tr role='row'><td>" + item.Nombre + "</td><td>$" + item.Salario + "</td><td><buttom id='editRol-" + indice + "' class='btn btn-primary' data-toggle='modal' data-target='.editar-rol'>EDITAR</buttom></td></tr>");
            indice++;
            return indice;
        });

        for (var i = 0; i < filas.children.length; i++) {
            DatosEditar(document.getElementById("editRol-" + i), datos[i]);
        }

        tabla = $("#tablaRoles").DataTable();
    }

    function DatosEditar(control, registro) {
        control.addEventListener("click", function (e) {
            var contenedor = document.getElementById("editCampo");
            contenedor.innerHTML = '<div class="controles"><label>Nombre rol:</label><input type="text" class="usuario control" value="' + registro.Nombre + '" readonly /></div><div class="controles"><input type="text" name="salario" value=" ' + registro.Salario + '" class="textbox" id="salarioEdit"><div class="texto noplaceholder">Salario rol</div></div>';
            EventosControles();
            var espacioEdit = document.getElementById("espacioBtnEdit");
            espacioEdit.innerHTML = '<button id="editarRol" class="btnEnviar" type="button">Guardar</button>';
            var editarControl = document.getElementById("editarRol");
            editarControl.addEventListener("click", function (e) {
                var ControlEditSalario = document.getElementById("salarioEdit");
                Moneda(ControlEditSalario);
                var alertas = document.getElementById("alertasEdit");
                if (EsDecimal(ControlEditSalario.value) === true) {
                    if (parseFloat(ControlEditSalario.value) > 0) {
                        salarioRol = parseFloat(ControlEditSalario.value);
                        var RolDTO = {id: registro.ID, salario: salarioRol};
                        $.ajax({
                            method: "POST",
                            url: "/rol/editar",
                            data: RolDTO,
                            Cache: false,
                            success: function (Resultado) {
                                if (Resultado === "OK") {
                                    MsgAgregado("El rol ha sido actualizado con éxito");
                                    OcultarVentana(".editar-rol");
                                    recuperarDatos();
                                }
                                else {
                                    MsgNoAgregado(Resultado);
                                }
                            }
                        });
                    }
                    else {
                        MensajeError(alertas, "El monto del salario no puede ser cero");
                    }
                }
                else {
                    MensajeError(alertas, "Hay un error en el monto del salario");
                }
            });
        });
    }

    function recuperarDatos() {
        $.ajax({
            method: "GET",
            url: "/datos/mostrarroles",
            Cache: false,
            processData: false,
            dataType: "JSON",
            success: function (resultado) {
                cargarDatos(resultado);
            }
        });
    }

    var accesoControl = document.getElementById("acceso");

    recuperarDatos();

    LimitarTexto(nombre, 50, false);
    Moneda(salario);

    agregar.addEventListener("click", function (e) {
        alertas.innerHTML = "";
        var nombreRol, salarioRol;
        var unicoControl = document.getElementById("unico");
        nombreRol = null;
        salarioRol = null;
        if (TieneContenido(nombre.value.trim())) {
            if (datos.length > 0) {
                var buscarRol = null;
                buscarRol = datos.find((rol) => rol.Nombre.toUpperCase() === nombre.value.trim().toUpperCase());
                nombreRol = (buscarRol === undefined) ? nombre.value.trim().toUpperCase() : null;
                if (nombreRol === null) {
                    MensajeError(alertas, "Ya existe un registro con este nombre");
                }
            }
            else {
                nombreRol = nombre.value.trim().toUpperCase();
            }
        }
        else {
            MensajeError(alertas, "Ingresar nombre del rol");
        }

        if (EsDecimal(salario.value) === true) {
            if (parseFloat(salario.value) > 0) {
                salarioRol = parseFloat(salario.value);
            }
            else {
                MensajeError(alertas, "El monto del salario no puede ser cero");
            }
        }
        else {
            MensajeError(alertas, "Hay un error en el monto del salario");
        }

        if (nombreRol !== null && salarioRol !== null) {
            var RolDTO = {Nombre: nombreRol, Salario: salarioRol, Acceso: parseInt(accesoControl.value), Unico: parseInt(unicoControl.value)};
            $.ajax({
                method: "POST",
                url: "/rol/agregar",
                data: RolDTO,
                Cache: false,
                success: function (Resultado) {
                    if (Resultado === "OK") {
                        MsgAgregado("El rol ha sido agregado con éxito");
                        LimpiarTextBox(".textbox", agregar);
                        OcultarVentana(".agregar-rol");
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
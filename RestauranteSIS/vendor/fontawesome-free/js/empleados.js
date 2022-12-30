$(document).ready(function () {
    var datos;
    var EstadoApp = {
        registros: []
    };

    var nombreControl = $("input")[0];
    var emailControl = $("input")[5];
    var passwordControl = document.getElementById("passwd");
    var generoControl = document.getElementById("genero");
    var duiControl = document.getElementById("dui");
    var nitControl = document.getElementById("nit");
    var imagenControl = document.getElementById("archivo");
    var direccionControl = document.getElementById("dir");
    var telefonoControl = document.getElementById("telefono");
    var sucursalControl = document.getElementById("sucursal");
    var usuarioControl = document.getElementById("usuario");
    var rolControl = document.getElementById("rol");
    var nacimientoControl = document.getElementById("fecha");

    CargarImagen("archivo", "visualizar");

    function CargarCombo(control, dataCombo) {
        control.innerHTML = "";
        dataCombo.map((dato) => {
            control.innerHTML += '<option value="' + dato.ID + '">' + dato.Nombre + '</option>';
        });
    }

    function CargarDatos(registros) {
        datos = registros;
        
        var datosSucursal = datos[0].Sucursales.filter((dato) => dato.Eliminar === 0);
        
        CargarCombo(sucursalControl, datosSucursal);

        CargarCombo(rolControl, datos[0].Roles);

        var tabla = $("#tablaEmpleados").DataTable();
        tabla.destroy();

        var filas = document.getElementById("cTablaEmpleados");

        filas.innerHTML = "";

        datos[0].RelEmpleadoSucursales.map((item) => {
            var valorEmpleado = datos[0].Empleados.find((dato) => dato.ID === item.EmpleadoID);
            var valorSucursal = datos[0].Sucursales.find((dato) => dato.ID === item.SucursalID);
            var valorRol = datos[0].Roles.find((dato) => dato.ID === item.RolID);
            var valorEstado = datos[0].Estados.find((dato) => dato.ID === item.EstadoID);
            
            if (EstadoApp.registros.length === 0) {
                EstadoApp.registros.push({
                    EmpleadoID: item.EmpleadoID,
                    SucursalID: item.SucursalID,
                    RolID: item.RolID,
                    EstadoID: item.EstadoID,
                    FechaIngreso: item.FechaIngreso,
                    FechaSalida: item.FechaSalida,
                    DatosEmpleado: valorEmpleado,
                    DatosSucursal: valorSucursal,
                    DatosRoles: valorRol,
                    DatosEstado: valorEstado
                });
            }
            else {
                var posicion = EstadoApp.registros.findIndex((dato) => dato.EmpleadoID === item.EmpleadoID);

                if (posicion === -1) {
                    EstadoApp.registros.push({
                        EmpleadoID: item.EmpleadoID,
                        SucursalID: item.SucursalID,
                        RolID: item.RolID,
                        EstadoID: item.EstadoID,
                        FechaIngreso: item.FechaIngreso,
                        FechaSalida: item.FechaSalida,
                        DatosEmpleado: valorEmpleado,
                        DatosSucursal: valorSucursal,
                        DatosRoles: valorRol,
                        DatosEstado: valorEstado
                    });
                }
                else {
                    if (item.FechaRetiro === null || Date(item.FechaRetiro) > Date(EstadoApp.registros.FechaRetiro)) {
                        EstadoApp.registros.splice(posicion, 1, {
                            EmpleadoID: item.EmpleadoID,
                            SucursalID: item.SucursalID,
                            RolID: item.RolID,
                            EstadoID: item.EstadoID,
                            FechaIngreso: item.FechaIngreso,
                            FechaSalida: item.FechaSalida,
                            DatosEmpleado: valorEmpleado,
                            DatosSucursal: valorSucursal,
                            DatosRoles: valorRol,
                            DatosEstado: valorEstado
                        });
                    }                    
                }
            }
        });

        var contador = 0;
        EstadoApp.registros.map((campo) => {
            if (campo.DatosEstado.ID === 1) {
                filas.innerHTML += '<tr><td>' + campo.DatosEmpleado.Nombre + '</td><td>' + campo.DatosEmpleado.Genero + '</td><td>' + campo.DatosRoles.Nombre + '</td><td>$' + campo.DatosRoles.Salario + '</td><td>' + campo.DatosSucursal.Nombre + '</td><td class="badge badge-success mt-3">' + campo.DatosEstado.Nombre + '</td><td><buttom class="btn btn-info" id="verReporte' + contador + '">Ver</buttom><buttom id="editEmpleado-' + contador + '" class="btn btn-primary ml-1" data-toggle="modal" data-target=".editar-empleados">Editar</buttom></td></tr>';
            }
            else {
                filas.innerHTML += '<tr><td>' + campo.DatosEmpleado.Nombre + '</td><td>' + campo.DatosEmpleado.Genero + '</td><td>' + campo.DatosRoles.Nombre + '</td><td>$' + campo.DatosRoles.Salario + '</td><td>' + campo.DatosSucursal.Nombre + '</td><td class="badge badge-danger mt-3">' + campo.DatosEstado.Nombre + '</td><td><buttom class="btn btn-info" id="verReporte' + contador + '">Ver</buttom><buttom id="editEmpleado-' + contador + '" class="btn btn-primary ml-1" data-toggle="modal" data-target=".editar-empleados">Editar</buttom></td></tr>';
            }            
            contador++;
        });

        for (var i = 0; i < EstadoApp.registros.length; i++) {
            var DataReporte = {
                EmpleadoDTO: EstadoApp.registros[i].DatosEmpleado,
                EstadoDTO: EstadoApp.registros[i].DatosEstado
            };
            VisualizarReporte(document.getElementById("verReporte" + i), "/empleado/reporte", DataReporte, document.getElementById("reporte"), "verEmpleado");
            ActualizarEmpleado(document.getElementById("editEmpleado-" + i), EstadoApp.registros[i]);
        }

        tabla = $("#tablaEmpleados").DataTable();
    }

    function RecuperarDatos() {
        $.ajax({
            method: "GET",
            url: "/datos/mostrarempleados",
            Cache: false,
            processData: false,
            dataType: "JSON",
            success: function (resultado) {
                CargarDatos(resultado);
            }
        });
    }

    //Validar rol
    function ValidarRol(id, rolID) {
        var validar;
        if (EstadoApp.registros.length > 0) {
            var idTemp = datos[0].Roles.find((item) => item.ID === parseInt(rolID) && item.Unico === 0);
            if (idTemp !== undefined) {
                validar = true;
            }
            else {
                var buscar = EstadoApp.registros.find((dato) => dato.RolID === parseInt(rolID) && dato.DatosRoles.Unico === 1);
                if (buscar !== undefined) {
                    if (buscar.EmpleadoID === id) {
                        validar = true;
                    }
                    else {
                        validar = false;
                    }
                }
                else {
                    validar = true;
                }
            }            
        }
        else {
            validar = true;
        }
        return validar;
    }

    RecuperarDatos();
        
    LimitarTexto(nombreControl, 75, true);

    nombreControl.addEventListener("keyup", function (e) {
        var nombreVector = nombreControl.value.trim().split(" ");
        var userName = "";

        if (nombreControl.value.trim().length > 0) {
            if (nombreVector.length > 0) {
                if (nombreVector.length === 1) {
                    userName = nombreVector[0].trim().toLowerCase();
                }
                else {
                    userName = nombreVector[0].trim().toLowerCase() + "." + nombreVector[1].trim().toLowerCase();
                }
            }
            var buscarUsuario = datos[0].Empleados.filter((empleado) => BuscarNombreUsuario(empleado.Usuario.toLowerCase()) === userName);
            console.log(buscarUsuario);
            userName = (buscarUsuario.length === 0) ? userName + "1" : userName + (buscarUsuario.length + 1);

            usuarioControl.value = userName;
            passwordControl.value = userName;
            document.getElementById("passwdplace").className = "texto noplaceholder";
        }
        else {
            usuarioControl.value = "";
            passwordControl.value = "";
            document.getElementById("passwdplace").className = "texto placeholder";
        }
    });

    emailControl.addEventListener("keypress", function (e) {
        var expresion = /^[ ]+$/;
        if (expresion.test(e.key) === true || emailControl.value.length >= 50) {
            e.preventDefault();
        }
    });
    Password(passwordControl, 30);


    //Agregar empleado
    document.getElementById("guardar").addEventListener("click", function (e) {
        var correoDomain = /^\w+([\.\+\-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,4})+$/;
        var nombre, genero, dui, nit, telefono, direccion, email, usuario, password, foto, fecha, sucursalID, rolID;
        var alertas = document.getElementById("alertas");
        alertas.innerHTML = "";

        genero = generoControl.value;
        sucursalID = parseInt(sucursalControl.value);
        if (ValidarRol(0, rolControl.value) === true) {
            rolID = parseInt(rolControl.value);
        }
        else {
            MensajeError(alertas, "El rol del empleado ha sido asignado a otro empleado");
        }

        if (nombreControl.value.trim().length > 0) {
            if (ValidarTexto(nombreControl.value.trim() === true)) {
                nombre = nombreControl.value.trim().toUpperCase();
            }
            else {
                MensajeError(alertas, "Error en el nombre del empleado");
            }
        }
        else {
            MensajeError(alertas, "El campo del nombre no puede quedar vacio");
        }

        if (duiControl.value.trim().length === 10) {
            var buscarDUI = datos[0].Empleados.find((dato) => dato.DUI === duiControl.value.trim());
            dui = (buscarDUI === undefined) ? duiControl.value.trim() : undefined;

            if (dui === undefined) {
                MensajeError(alertas, "El numero de DUI que ha ingresado ya ha sido utilizado");
            }
        }
        else {
            MensajeError(alertas, "Error en el número de DUI");
        }

        if (nitControl.value.trim().length === 17) {
            if (ValidarFechaNIT(nitControl.value.trim())) {
                var buscarNIT = datos[0].Empleados.find((dato) => dato.NIT === nitControl.value.trim());
                nit = (buscarNIT === undefined) ? nitControl.value.trim() : undefined;

                if (nit === undefined) {
                    MensajeError(alertas, "El número de NIT ya ha sido utilizado");
                }
            }
            else {
                MensajeError(alertas, "Número de NIT no válido");
            }            
        }
        else {
            MensajeError(alertas, "Error en el número de nit");
        }

        if (emailControl.value.trim().length > 0 && emailControl.value.trim().length <= 50) {
            if (correoDomain.test(emailControl.value.trim())) {
                var buscarEmail = datos[0].Empleados.find((dato) => dato.Email === emailControl.value.trim().toLowerCase());
                email = (buscarEmail === undefined) ? emailControl.value.trim().toLowerCase() : undefined;
                if (email === undefined) {
                    MensajeError(alertas, "El correo electrónico es usado por otro empleado");
                }
            }
            else {
                MensajeError(alertas, "Error en el correo ingresado");
            }
        }
        else {
            email = "";
        }

        if (TieneContenido(telefonoControl.value.trim()) && telefonoControl.value.trim().length === 9) {
            var buscarTelefono = datos[0].Empleados.find((dato) => dato.Telefono === telefonoControl.value.trim());
            telefono = (buscarTelefono === undefined) ? telefonoControl.value.trim() : undefined;
            if (telefono === undefined) {
                MensajeError(alertas, "El número de telefono es usado por otro empleado");
            }
        }
        else {
            MensajeError(alertas, "Error en el número de telefono");
        }

        if (passwordControl.value.trim().length > 0 && usuarioControl.value.trim().length > 0) {
            password = passwordControl.value.trim();
            usuario = usuarioControl.value.trim().toLowerCase();
        }

        if (nacimientoControl.value !== undefined) {
            fecha = nacimientoControl.value.trim();
        }
        else {
            MensajeError(alertas, "Seleccionar una fecha de nacimiento");
        }

        if (TieneContenido(direccionControl.value.trim())) {
            direccion = direccionControl.value.trim();
        }
        else {
            MensajeError(alertas, "Ingresar la dirección del empleado");
        }

        if (TieneContenido(imagenControl.value)) {
            foto = imagenControl.value;
        }
        else {
            MensajeError(alertas, "Adjuntar foto del empleado");
        }

        if (Date.parse(nacimientoControl.value)) {
            var fechaActual = new Date();
            var year = fechaActual.getFullYear() - 18;
            var edad = new Date(nacimientoControl.value);
            if (edad.getFullYear() > year) {
                MensajeError(alertas, "No se puede registrar a una persona menor de 18 años");
                fecha = "";
            }
            else {
                fecha = nacimientoControl.value;
            }
        }
        else {
            MensajeError(alertas, "Error en la fecha");
        }

        if (nombre !== undefined && usuario !== undefined && telefono !== undefined && password !== undefined && nit !== undefined && dui !== undefined && direccion !== undefined && foto !== undefined && fecha !== "" && rolID !== undefined) {
            var DataDTO = {
                EmpleadoSucursalDTO: {
                    SucursalID: sucursalID,
                    RolID: parseInt(rolID),
                    EstadoID: 1
                },
                EmpleadoDTO: {
                    Nombre: nombre,
                    Genero: genero,
                    DUI: dui,
                    NIT: nit,
                    Telefono: telefono,
                    Direccion: direccion,
                    Email: email,
                    Usuario: usuario,
                    Password: password,
                    FechaNacimiento: fecha,
                    Foto: foto
                }
            };

            var formulario = new FormData();
            formulario.append("Nombre", usuario);
            formulario.append("Foto", imagenControl.files[0]);

            $.ajax({
                method: "POST",
                url: "/empleado/subirfoto",
                data: formulario,
                cache: false,
                processData: false,
                contentType: false,
                success: function (Resultado) {
                    if (Resultado !== "ERROR") {
                        DataDTO.EmpleadoDTO.Foto = Resultado;
                        $.ajax({
                            method: "POST",
                            url: "/empleado/agregar",
                            data: DataDTO,
                            cache: false,
                            success: function (rst) {
                                if (rst === "OK") {
                                    MsgAgregado("El empleado se registró con éxito");
                                    LimpiarControl(direccionControl);
                                    LimpiarTextBox(".textbox", document.getElementById("guardar"));
                                    LimpiarControl(nacimientoControl);
                                    LimpiarControl(usuarioControl);
                                    OcultarImagen("archivo", imagenControl.files[0], document.getElementById("visualizar"));
                                    OcultarVentana(document.getElementById("agregarEmpleado"));
                                    RecuperarDatos();
                                }
                                else {
                                    MsgNoAgregado(rst);
                                }
                            }
                        });
                    }
                    else {
                        MsgNoAgregado("Error al subir la imágen");
                    }
                }
            });
        }
    });
    
    //Evento de actualizacion de datos.
    function ActualizarEmpleado(boton, dataEdit) {
        var formulario = document.getElementById("formEditar");
        boton.addEventListener("click", function (e) {
            formulario.innerHTML = '<div class="modal-header"><h3 class="modal-title" id="myLargeModalLabel">DATOS EMPLEADO</h3> <button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">×</span></button></div><div class="grupoControles"><div class="controles"><label>Nombre:</label><input type="text" name="nombre" class="usuario control" id="nombreEdit" value="' + dataEdit.DatosEmpleado.Nombre + '" readonly></div><div class="controles"><input type="text" name="telefono" class="textbox" id="telefonoEdit" value="' + dataEdit.DatosEmpleado.Telefono + '"><div class="texto noplaceholder">Telefono empleado</div></div></div><div class="grupoControles"><div class="controles"><input type="file" name="imagen" id="archivoEdit" value="' + dataEdit.DatosEmpleado.Foto + '"><label for="archivoEdit" class="archivo">Seleccionar imágen</label><article id="visualizarEdit" class="ocultarArchivo"></article></div><div class="controles"><div class="textoarea noplaceholderareas">Dirección empleado</div><textarea class="area" name="direccion" id="dirEdit">' + dataEdit.DatosEmpleado.Direccion + '</textarea></div></div><div class="grupoControles"><div class="controles"><label for="sucursal">Estado:</label><select name="sucursal" class="control" id="estadoEdit"></select></div><div class="controles"><input type="email" name="emailEdit" class="textbox" id="emailEdit" value="' + dataEdit.DatosEmpleado.Email + '"><div class="texto' + ((dataEdit.DatosEmpleado.Email === "" || dataEdit.DatosEmpleado.Email === null) ? '' : ' noplaceholder') + '">E-mail empleado</div></div></div><div class="grupoControles"><div class="controles controlesUsuario"><label for="usuario">Usuario:</label><input type="text" name="usuario" value="' + dataEdit.DatosEmpleado.Usuario + '" id="usuarioEdit" class="usuario control" readonly /></div><div class="controles"><label>Contraseña:</label><input type="password" id="passwdEdit" name="passwd" class="usuario control" style="border-radius: 0 0 0 0;" readonly><button type="button" id="restartPassword" class="password"><i class="fa fa-eraser"></i></button></div></div><div class="grupoControles"><div class="controles"><label for="sucursal">Sucursal:</label><select name="sucursal" class="control" id="sucursalEdit"></select></div><div class="controles"><label for="rol">rol:</label><select name="rol" class="control" id="rolEdit"></select></div></div><div class="grupoControles"><button id="editar" class="btnEnviar" type="button">Guardar</button></div><div id="alertasEditar"></div>';
            EventosControles();

            CargarImagen("archivoEdit", "visualizarEdit");

            var vista = document.getElementById("visualizarEdit");

            document.getElementById("restartPassword").addEventListener("click", function (e) {
                document.getElementById("passwdEdit").value = dataEdit.DatosEmpleado.Usuario.toLowerCase();
            });//------------------------------------------------------------------------------------------------------------------------

            var estadoControlEdit = document.getElementById("estadoEdit");
            var sucursalControlEdit = document.getElementById("sucursalEdit");
            var rolControlEdit = document.getElementById("rolEdit");

            vista.setAttribute("style", "background-image: url(" + dataEdit.DatosEmpleado.Foto + ");");
            vista.className = "mostrarArchivo";
            CargarCombo(estadoControlEdit, datos[0].Estados);
            var datosSucursal = datos[0].Sucursales.filter((dato) => dato.Eliminar === 0);
            CargarCombo(sucursalControlEdit, datosSucursal);
            CargarCombo(rolControlEdit, datos[0].Roles);

            estadoControlEdit.value = dataEdit.EstadoID;
            sucursalControlEdit.value = dataEdit.SucursalID;
            rolControlEdit.value = dataEdit.RolID;

            EditarEmpleado(document.getElementById("editar"), dataEdit);
        });
    }

    function EditarEmpleado(boton, dataEmpleadoEdit) {
        boton.addEventListener("click", function (e) {
            var telEditControl = document.getElementById("telefonoEdit");
            var emailEditControl = document.getElementById("emailEdit");
            var passEditControl = document.getElementById("passwdEdit");
            var fotoEditControl = document.getElementById("archivoEdit");
            var dirEditControl = document.getElementById("dirEdit");
            var passEdit, telEdit, emailEdit, fotoEdit, dirEdit;
            var sucursalEdit, rolEdit, estadoEdit;
            var alertas = document.getElementById("alertasEditar");
            alertas.innerHTML = "";

            EventosControles();

            if (document.getElementById("passwdEdit").value.trim().length > 0) {
                passEdit = dataEmpleadoEdit.DatosEmpleado.Usuario.toLowerCase();
            }

            sucursalEdit = parseInt(document.getElementById("sucursalEdit").value);

            if (ValidarRol(dataEmpleadoEdit.EmpleadoID, document.getElementById("rolEdit").value)) {
                rolEdit = parseInt(document.getElementById("rolEdit").value);
            }
            else {
                MensajeError(alertas, "El rol ha sido asignado a otro empleado");
            }
            
            estadoEdit = parseInt(document.getElementById("estadoEdit").value);

            if (TieneContenido(telEditControl.value.trim()) && telEditControl.value.length === 9) {
                var buscarTel = datos[0].Empleados.find((empleado) => empleado.Telefono === telEditControl.value && empleado.ID !== dataEmpleadoEdit.EmpleadoID);
                telEdit = (buscarTel === undefined) ? telEditControl.value : undefined;
                if (telEdit === undefined) {
                    MensajeError(alertas, "El número de telefono ya ha sido asignado a otro empleado");
                }
            }
            else {
                MensajeError(alertas, "Error en el telefono");
            }

            if (TieneContenido(emailEditControl.value.trim())) {
                buscarEmail = datos[0].Empleados.find((Empleado) => Empleado.email === emailEditControl.value.trim() && Empleado.ID !== dataEmpleadoEdit.EmpleadoID);
                if (buscarEmail === undefined) {
                    emailEdit = emailEditControl.value.trim();
                }
                else {
                    MensajeError(alertas, "El email lo posee otro empleado");
                }
            }

            if (TieneContenido(fotoEditControl.getAttribute("value"))) {
                fotoEdit = fotoEditControl.getAttribute("value");
            }
            else {
                MensajeError(alertas, "Agregar una foto para el empleado");
            }

            if (TieneContenido(dirEditControl.value.trim())) {
                dirEdit = dirEditControl.value.trim();
            }
            else {
                MensajeError(alertas, "Agregar la dirección del empleado");
            }

            if (telEdit !== undefined && dirEdit !== undefined && fotoEdit !== undefined && rolEdit !== undefined) {
                var formDataEdit = new FormData();
                formDataEdit.append("Nombre", dataEmpleadoEdit.DatosEmpleado.Usuario);
                formDataEdit.append("Foto", fotoEditControl.files[0]);
                var actualizarRel = false;

                if (rolEdit !== dataEmpleadoEdit.RolID || sucursalEdit !== dataEmpleadoEdit.SucursalID || estadoEdit !== dataEmpleadoEdit.EstadoID) {
                    actualizarRel = true;
                }

                var DataDTO = {
                    RelDTO: {
                        EmpleadoID: dataEmpleadoEdit.EmpleadoID,
                        SucursalID: sucursalEdit,
                        RolID: parseInt(rolEdit),
                        EstadoID: estadoEdit
                    },
                    EmpleadoDTO: {
                        ID: dataEmpleadoEdit.EmpleadoID,
                        Telefono: telEdit,
                        Direccion: dirEdit,
                        Email: emailEdit,
                        Password: passEdit,
                        Foto: fotoEdit
                    },
                    ActualizarREl: actualizarRel
                };
                if (fotoEdit !== fotoEditControl.value && fotoEditControl.files[0] !== undefined) {
                    $.ajax({
                        method: "POST",
                        url: "/empleado/subirfoto",
                        data: formDataEdit,
                        cache: false,
                        processData: false,
                        contentType: false,
                        success: function (Resultado) {
                            if (Resultado === "ERROR") {
                                MsgNoAgregado("Error al subir la imágen");
                                return false;
                            }
                        }
                    });
                }

                $.ajax({
                    method: "POST",
                    url: "/empleado/editar",
                    data: DataDTO,
                    cache: false,
                    success: function (Rst) {
                        if (Rst === "OK") {                            
                            MsgAgregado("Los datos del empleado han sido actualizados");
                            OcultarVentana(document.getElementById("editarEmpleado"));
                            RecuperarDatos();
                        }
                        else {
                            MsgNoAgregado(Rst);
                        }
                    }
                });
            }
        });
    }
});
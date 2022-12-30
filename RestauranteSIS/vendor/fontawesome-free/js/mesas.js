$(document).ready(function (e) {
    var datos;
    var MesasEstado = [];

    //Asignar árbol de estado
    function listarMesas() {
        $.ajax({
            method: "GET",
            url: "/mesas/cantidadmesas",
            cache: false,
            dataType: "JSON",
            success: function (resultado) {
                if (resultado !== null || resultado !== undefined) {
                    MesasEstado = resultado;
                    CargarMesas();
                }
            }
        });
    }

    listarMesas();

    recuperarDatos();

    function iniciarTimer() {
        setInterval(function (e) {
            if (datos[0].Reservaciones.length > 0) {
                var fechaActual = new Date();
                datos[0].Reservaciones.map((r) => {
                    var registroMesa = MesasEstado.find((m) => m.ID === r.MesaID);
                    var posicion = MesasEstado.findIndex((m) => m.ID === r.MesaID);
                    var registroCliente = datos[0].Clientes.find((c) => c.ID === r.clienteID);
                    if (registroMesa !== undefined) {
                        var fecha = new Date(r.Fecha);
                        if (fechaActual.getFullYear() === fecha.getFullYear() && fechaActual.getMonth() === fecha.getMonth() && fechaActual.getDate() === fecha.getDate() && fechaActual.getHours() === (fecha.getHours() - 1) && registroMesa.Ocupado === 0) {
                            registroMesa.Ocupado = 2;
                            registroMesa.Cliente = registroCliente;
                            MesasEstado.splice(posicion, 1, registroMesa);
                            CargarMesas();
                        }
                    }
                });
            }
        }, 3000);
    }

    //CargarMesas
    function CargarMesas() {
        var mesas = document.getElementById("mesas");
        mesas.innerHTML = "";
        var indice = 0;
        MesasEstado.map((mesa) => {
            if (mesa.Ocupado === 0) {
                mesas.innerHTML += '<div><h3 class="encabezadoMesa">DISPONIBLE</h3><div class="cuerpoMesa"><div class="tituloMesa disponible"><h1 class="numeroMesa">' + mesa.Numero + '</h1></div><button type="button" id="mesa-' + indice + '">+</button></div></div>';
            }
            else if (mesa.Ocupado === 1) {
                mesas.innerHTML += '<div><h3 class="encabezadoMesa">OCUPADO</h3><div class="cuerpoMesa"><div class="tituloMesa ocupado"><h1 class="numeroMesa">' + mesa.Numero + '</h1></div><button type="button" id="mesa-' + indice + '">+</button></div></div>';
            }
            else {
                mesas.innerHTML += '<div><h3 class="encabezadoMesa">RESERVADO</h3><div class="cuerpoMesa"><div class="tituloMesa reservado"><h1 class="numeroMesa">' + mesa.Numero + '</h1></div><button type="button" id="mesa-' + indice + '">+</button></div></div>';
            }
            indice++;
        });
        for (var i = 0; i < MesasEstado.length; i++) {
            if (MesasEstado[i].Ocupado === 0) {
                ventanaAsignar(document.getElementById("mesa-" + i), MesasEstado[i]);
            }
            else if (MesasEstado[i].Ocupado === 1) {
                ventanaEditar(document.getElementById("mesa-" + i), MesasEstado[i]);
            }
            else {
                ventanaReservada(document.getElementById("mesa-" + i), MesasEstado[i]);
            }
        }
    }

    //Recurperar datos
    function recuperarDatos() {
        $.ajax({
            method: "GET",
            url: "/datos/clientesreservacion",
            cache: false,
            dataType: "JSON",
            success: function (resultado) {
                if (resultado !== null || resultado !== undefined) {
                    datos = resultado;
                    iniciarTimer();
                }
            }
        });
    }

    //Modal asignar
    function ventanaAsignar(boton, data) {
        boton.addEventListener("click", function (e) {
            $("#asignarMesa").modal("show");
            FormAsignar(0, data);
        });
    }
    //Modal editar
    function ventanaEditar(boton, data) {
        boton.addEventListener("click", function (e) {
            $("#editarMesa").modal("show");
            formularioEditar(data);
        });
    }
    //Modal reservada
    function ventanaReservada(boton, data) {
        boton.addEventListener("click", function (e) {
            $("#verMesa").modal("show");
        });
    }

    //Eventos de autocompletado de texto
    function AutocompletarContenido() {
        var completarControl = document.getElementById("autoCompletar");
        OcultarCompletar(completarControl);

        document.getElementById("nombreCliente").addEventListener("keydown", function (e) {
            if (e.keyCode === 13) {
                e.preventDefault();
            }
        });

        document.getElementById("nombreCliente").addEventListener("keyup", function (e) {
            completarControl.innerHTML = "";
            var dataClientes = [];

            document.getElementById("nit").value = "";
            document.getElementById("dui").value = "";
            document.getElementById("telefono").value = "";
            if (datos.length > 0 && document.getElementById("nombreCliente").value.length > 0) {
                for (var i = 0; i < datos[0].Clientes.length; i++) {
                    if (datos[0].Clientes[i].Nombre.toLowerCase().substr(0, document.getElementById("nombreCliente").value.trim().length) === document.getElementById("nombreCliente").value.trim().toLowerCase()) {
                        completarControl.innerHTML += '<button id="opcion-' + i + '" type="button" class="opcionCompletar">' + datos[0].Clientes[i].Nombre + '</button>';
                        dataClientes[dataClientes.length] = datos[0].Clientes[i]
                    }
                    if (datos[0].Clientes[i].Nombre.toLowerCase().substr(0, document.getElementById("nombreCliente").value.trim().length) === datos[0].Clientes[i].Nombre.toLowerCase()) {
                        completarControl.innerHTML = "";
                    }
                }

                for (var i = 0; i < completarControl.children.length; i++) {
                    Navegar(completarControl.children[i], dataClientes[i]);
                    SeleccionarNombre(completarControl.children[i], dataClientes[i]);
                }
                completarControl.setAttribute("style", "display: block;");
            }
            else {
                OcultarCompletar(completarControl);
            }
            if (completarControl.children.length > 0) {
                completarControl.setAttribute("style", "display: block;");
            }
            else {
                OcultarCompletar(completarControl);
            }
            if (e.keyCode === 40 && completarControl.children.length > 0) {
                completarControl.firstChild.focus();
            }
        });
    }

    function SeleccionarNombre(control, dataCliente) {
        control.addEventListener("click", function (e) {
            document.getElementById("nombreCliente").value = dataCliente.Nombre;
            document.getElementById("nit").value = dataCliente.NIT;
            document.getElementById("dui").value = dataCliente.DUI;
            document.getElementById("telefono").value = dataCliente.Telefono;
            OcultarCompletar(control.parentNode);
            control.parentNode.innerHTML = "";
        });
    }

    function Navegar(control, dataCliente) {
        control.addEventListener("keydown", function (e) {
            if (e.keyCode === 38) {
                control.previousSibling.focus();
            }
            else if (e.keyCode === 40) {
                control.nextSibling.focus();
            }
            else if (e.keyCode === 13) {
                document.getElementById("nombreCliente").value = dataCliente.Nombre;
                document.getElementById("nit").value = dataCliente.NIT;
                document.getElementById("dui").value = dataCliente.DUI;
                document.getElementById("telefono").value = dataCliente.Telefono;
                OcultarCompletar(control.parentNode);
                control.parentNode.innerHTML = "";
            }
        });

        control.addEventListener("mouseover", function (e) {
            this.focus();
        });
    }

    function OcultarCompletar(control) {
        control.setAttribute("style", "display: none;");
    }

    //Crear formulario para asignar mesa a asignar
    function FormAsignar(opcion, data) {
        document.getElementById("formAsignarMesa").innerHTML = '<div class="modal-header"><h3 class="modal-title" id="myLargeModalLabel">ASIGNAR MESA</h3><button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">×</span></button></div><div class="grupoControles" id="sectionCliente"><div id="campoCliente" class="controles"></div><div id="control1" class="controles"></div></div><div class="grupoControles" id="controles2"></div><div class="grupoControles"><div id="botonesPrincipales" class="controles" style="width: 100%;"></div><div id="otrosControles" class="controles" style="width: 100%;"></div></div><div id="alertasAsignar"></div>';

        if (opcion === 1) {
            document.getElementById("campoCliente").innerHTML = '<input type="text" id="nombre" name="nombreCliente" class="textbox" style="border-radius: 0 0 0 0;"><div class="texto">Nombre cliente</div>';
            document.getElementById("control1").innerHTML = '<input type="text" name="nit" class="textbox" id="nit"><div class="texto">NIT cliente</div>';
            document.getElementById("controles2").innerHTML = '<div class="controles"><input type="text" name="dui" class="textbox" id="dui"><div class="texto">DUI cliente</div></div><div class="controles"><input type="text" name="telefono" class="textbox" id="telefono"><div class="texto">Telefono cliente</div></div>';
            document.getElementById("otrosControles").innerHTML = '<button id="agregar" type="button" class="btnEnviar" style="margin-right: 10px;">Agregar</button><button id="cancelar" type="button" class="btnEnviar">Cancelar</button>';
            document.getElementById("botonesPrincipales").innerHTML = "";
            document.getElementById("botonesPrincipales").setAttribute("style", "display: none;");
            EventosControles();
            LimitarTexto(document.getElementById("nombre"), 75, true);
            document.getElementById("cancelar").addEventListener("click", function (e) {
                FormAsignar(0, data);
            });

            agregarCliente(document.getElementById("agregar"), document.getElementById("nombre"), document.getElementById("nit"), document.getElementById("dui"), document.getElementById("telefono"), document.getElementById("alertasAsignar"), data);
        }
        else if (opcion === 2) {
            document.getElementById("formAsignarMesa").innerHTML = '<div class="modal-header"><h3 class="modal-title" id="myLargeModalLabel">ASIGNAR MESA</h3><button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">×</span></button></div><div class="grupoControles" id="sectionCliente"><div id="campoCliente" class="controles"></div><div id="control1" class="controles"></div></div><div class="grupoControles" id="controles2"></div><div class="grupoControles"><div class="controles"><label>Fecha:</label><input type="date" name="fechaAsignar" value="" class="control" id="fechaAsignar"/></div><div class="controles"><label>Hora:</label><input type="time" name="horaAsignar" value="" class="control" id="horaAsignar"/></div></div><div class="grupoControles"><div id="botonesPrincipales" class="controles" style="width: 100%;"></div><div id="otrosControles" class="controles" style="width: 100%;"></div></div><div id="alertasAsignar"></div>';

            document.getElementById("campoCliente").innerHTML = '<label>Cliente:</label><input type="text" id="nombreCliente" name="nombreCliente" class="control" style="border-radius: 0 0 0 0;"><button type="button" id="cambiarModoCliente" class="password"><i class="fa fa-user-edit"></i></button><article id="autoCompletar" class="autocompletar"></article>';
            document.getElementById("control1").innerHTML = '<label for="nit">NIT:</label><input type="text" name="nit" class="usuario control" id="nit" readonly>';
            document.getElementById("controles2").innerHTML = '<div class="controles"><label for="dui">DUI:</label><input type="text" name="dui" class="usuario control" id="dui" readonly></div><div class="controles"><label for="telefono">Telefono:</label><input type="text" name="telefono" class="usuario control" id="telefono" readonly></div>';
            document.getElementById("otrosControles").innerHTML = "";
            document.getElementById("botonesPrincipales").innerHTML = '<button id="programar" class="btnEnviar mr-2" type="button">Programar</button><button id="descartar" class="btnEnviar" type="button">Cancelar</button>';
            AutocompletarContenido();
            cambioModoCliente(document.getElementById("cambiarModoCliente"), data);
            document.getElementById("descartar").addEventListener("click", function (e) {
                FormAsignar(0, data);
            });
        }
        else {
            document.getElementById("campoCliente").innerHTML = '<label>Cliente:</label><input type="text" id="nombreCliente" name="nombreCliente" class="control" style="border-radius: 0 0 0 0;"><button type="button" id="cambiarModoCliente" class="password"><i class="fa fa-user-edit"></i></button><article id="autoCompletar" class="autocompletar"></article>';
            document.getElementById("control1").innerHTML = '<label for="nit">NIT:</label><input type="text" name="nit" class="usuario control" id="nit" readonly>';
            document.getElementById("controles2").innerHTML = '<div class="controles"><label for="dui">DUI:</label><input type="text" name="dui" class="usuario control" id="dui" readonly></div><div class="controles"><label for="telefono">Telefono:</label><input type="text" name="telefono" class="usuario control" id="telefono" readonly></div>';
            document.getElementById("otrosControles").innerHTML = "";
            //document.getElementById("botonesPrincipales").innerHTML = '<button id="asignar" class="btnEnviar mr-2" type="button">Asignar</button><button id="reservar" class="btnEnviar" type="button">Reservar</button>';
            document.getElementById("botonesPrincipales").innerHTML = '<button id="asignar" class="btnEnviar mr-2" type="button">Asignar</button>';
            AutocompletarContenido();
            cambioModoCliente(document.getElementById("cambiarModoCliente"), data);
            /*document.getElementById("reservar").addEventListener("click", function (e) {
                FormAsignar(2, data);
            });*/
            AsignarMesa(document.getElementById("asignar"), data);
        }
    }

    function cambioModoCliente(boton, data) {
        boton.addEventListener("click", function (e) {
            FormAsignar(1, data);
        });
    }

    //Botón agregar cliente
    function agregarCliente(boton, nombreControl, nitControl, duiControl, telControl, mensajes, data) {
        boton.addEventListener("click", function (e) {
            var nombre, nit, dui, telefono;

            var valido = true;

            mensajes.innerHTML = "";

            if (TieneContenido(nombreControl.value.trim())) {
                nombre = nombreControl.value.trim().toUpperCase();
            }
            else {
                MensajeError(mensajes, "Agregar el nombre del cliente");
            }

            if (TieneContenido(nitControl.value.trim()) && nitControl.value.trim().length === 17) {
                if (ValidarFechaNIT(nitControl.value.trim()) === true) {
                    if (datos[0].Clientes.length > 0) {
                        var buscarNIT = datos[0].Clientes.find((c) => c.NIT === nitControl.value.trim());
                        nit = (buscarNIT === undefined) ? nitControl.value.trim() : undefined;
                        if (nit === undefined) {
                            MensajeError(mensajes, "El número de NIT está asignado a otro cliente");
                        }
                    }
                    else {
                        nit = nitControl.value.trim();
                    }
                }
                else {
                    nit = undefined;
                    MensajeError(mensajes, "Número de NIT no válido");
                }
            }
            else {
                MensajeError(mensajes, "Error en el número de NIT");
            }

            if (TieneContenido(duiControl.value.trim()) && duiControl.value.trim().length === 10) {
                if (datos[0].Clientes.length > 0) {
                    buscarDUI = datos[0].Clientes.find((c) => c.DUI === duiControl.value.trim());
                    dui = (buscarDUI === undefined) ? duiControl.value.trim() : undefined;
                    if (dui === undefined) {
                        valido = false;
                        MensajeError(mensajes, "El número de DUI está asignado a otro cliente");
                    }
                }
                else {
                    dui = duiControl.value.trim();
                }
            }

            if (TieneContenido(telControl.value.trim()) && telControl.value.trim().length === 9) {
                telefono = telControl.value.trim();
            }

            if (nombre !== undefined && nit !== undefined && valido === true) {
                var clienteDTO = {
                    Nombre: nombre,
                    NIT: nit,
                    DUI: dui,
                    Telefono: telefono
                };
                $.ajax({
                    method: "POST",
                    url: "/cliente/agregar",
                    data: clienteDTO,
                    Cache: false,
                    success: function (Resultado) {
                        if (Resultado === "OK") {
                            MsgAgregado("El cliente ha sido agregado con éxito");
                            recuperarDatos();
                            FormAsignar(0, data);
                        }
                        else {
                            MsgNoAgregado(Resultado);
                        }
                    }
                });
            }
        });
    }
    //Actualizar datos en la variable de sesion
    function actualizarEstado(opcion) {
        $.ajax({
            method: "POST",
            url: "/mesas/asignarmesa",
            data: { Mesas: JSON.stringify(MesasEstado) },
            cache: false,
            dataType: "JSON",
            success: function (Resultado) {
                if (Resultado !== undefined || Resultado !== null) {
                    CargarMesas();
                    if (opcion === 1) {
                        MsgAgregado("Mesa asignada");
                        OcultarVentana("#asignarMesa");
                    }
                }
                else {
                    if (opcion === 1) {
                        MsgNoAgregado("Mesa no asignada");
                    }

                }
            }
        });
    }

    //Botón asignar
    function AsignarMesa(boton, data) {
        boton.addEventListener("click", function (e) {
            var clienteControl = document.getElementById("nombreCliente");
            var nitControl = document.getElementById("nit");
            var alertas = document.getElementById("alertasAsignar");

            alertas.innerHTML = "";

            if (clienteControl.value.trim().length > 0 && nitControl.value.trim().length > 0) {
                var registroCliente = datos[0].Clientes.find((c) => c.Nombre === clienteControl.value.trim().toUpperCase() && c.NIT === nitControl.value.trim());
                if (registroCliente !== undefined) {
                    data.Cliente = registroCliente;
                    data.Ocupado = 1;
                    var index = MesasEstado.findIndex((m) => m.ID === data.ID);
                    MesasEstado.splice(index, 1, data);
                    actualizarEstado(1);
                }
            }
            else {
                MensajeError(alertas, "Debe seleccionar un cliente registrado");
            }
        });
    }

    //Formulario para agregar platos al cliente
    function formularioEditar(data) {
        document.getElementById("formEditarMesa").innerHTML = '<div class="modal-header"><h3 class="modal-title" id="myLargeModalLabel">MESA OCUPADA</h3><button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">×</span></button></div><div class="grupoControles"><div class="controles"><label>Plato:</label><input type="text" id="plato" name="plato" class="control" style="border-radius: 0 0 0 0;"><button type="button" id="addPlato" class="password"><i class="fa fa-plus"></i></button><article id="platoCompletar" class="autocompletar"></article></div><div class="controles"><label>Precio:</label><input type="text" id="precio" name="precio" class="control" value="" readonly></div></div><table class="table table-striped"><thead class="table table-dark"><tr><th>PLATO</th><th>PRECIO</th><th>CANTIDAD</th><th>IMPORTE</th></tr></thead><tbody id="cuerpoTabla"></tbody></table><div class="grupoControles"><div class="controles"><label>Sub-Total:</label><input type="text" id="subtotal" name="subtotal" class="control" value="" readonly></div><div class="controles"><label>Descuento:</label><input type="text" id="descuento" name="descuento" class="control" value="" readonly></div></div><div class="grupoControles"><div class="controles"><label>IVA:</label><input type="text" id="iva" name="iva" class="control" value="" readonly></div><div class="controles"><label>Total:</label><input type="text" id="total" name="total" class="control" value="" readonly></div></div><div class="grupoControles"><div class="controles"><label for="consumidor" class="archivo">Consumidor final: <input type="radio" id="consumidor" name="factura" value="1" checked /></label></div><div class="controles"><label for="credito" class=archivo>Credito fiscal: <input type="radio" id="credito" name="factura" value="2" /></label></div></div><div class="grupoControles"><button id="facturar" class="btnEnviar" type="button">Facturar</button><button id="anularEdit" class="btnEnviar" type="button">Anular</button></div><div id="alertasEdit"></div>';
        generarFilas(document.getElementById("cuerpoTabla"), data);
        completarPlato(document.getElementById("platoCompletar"));
        agregarPlatoMesa(document.getElementById("addPlato"), data);
        quitarMesa(document.getElementById("anularEdit"), data);
        document.getElementById("consumidor").addEventListener("change", function (e) {
            generarFilas(document.getElementById("cuerpoTabla"), data);
        });
        document.getElementById("credito").addEventListener("change", function (e) {
            pagar(data);
            generarFilas(document.getElementById("cuerpoTabla"), data);
        });

        //-------------------------------------------------------------------------------------------------------------------------
        facturar(document.getElementById("facturar"), data);
    }

    //Quitar mesa
    function quitar(data) {
        data.Ocupado = 0;
        data.Cliente = {};
        data.Platos = [];
        actualizarEstado(0);
        OcultarVentana("#editarMesa");
    }

    //Evento para quitar mesa
    function quitarMesa(boton, data) {
        boton.addEventListener("click", function (e) {
            quitar(data);
        });
    }

    //Autocompletar formulario agregar platos
    function completarPlato(control) {
        OcultarCompletar(control);

        document.getElementById("plato").addEventListener("keydown", function (e) {
            if (e.keyCode === 13) {
                e.preventDefault();
            }
        });

        document.getElementById("plato").addEventListener("keyup", function (e) {
            control.innerHTML = "";

            document.getElementById("precio").value = "";

            var dataPlatos = [];

            if (datos.length > 0 && document.getElementById("plato").value.trim().length > 0) {
                for (var i = 0; i < datos[0].Platos.length; i++) {
                    if (datos[0].Platos[i].Nombre.toLowerCase().substr(0, document.getElementById("plato").value.trim().length) === document.getElementById("plato").value.trim().toLowerCase()) {
                        control.innerHTML += '<button id="platoOpcion-' + i + '" type="button" class="opcionCompletar">' + datos[0].Platos[i].Nombre + '</button>';
                        dataPlatos[dataPlatos.length] = datos[0].Platos[i];
                    }
                    if (datos[0].Platos[i].Nombre.toLowerCase().substr(0, document.getElementById("plato").value.trim().length) === datos[0].Platos[i].Nombre.toLowerCase()) {
                        control.innerHTML = "";
                    }
                }
                for (var i = 0; i < control.children.length; i++) {
                    NavegarPlato(control.children[i], dataPlatos[i]);
                    SeleccionarPlato(control.children[i], dataPlatos[i]);
                }
                control.setAttribute("style", "display: block;");
            }
            else {
                OcultarCompletar(control);
            }
            if (control.children.length > 0) {
                control.setAttribute("style", "display: block;");
            }
            else {
                OcultarCompletar(control);
            }
            if (e.keyCode === 40 && control.children.length > 0) {
                control.firstChild.focus();
            }
        });
    }

    function SeleccionarPlato(control, data) {
        control.addEventListener("click", function (e) {
            document.getElementById("plato").value = data.Nombre;
            document.getElementById("precio").value = data.Pagar;
            OcultarCompletar(control.parentNode);
            control.parentNode.innerHTML = "";
        });
    }

    function NavegarPlato(control, data) {
        control.addEventListener("keydown", function (e) {
            if (e.keyCode === 38) {
                control.previousSibling.focus();
            }
            else if (e.keyCode === 40) {
                control.nextSibling.focus();
            }
            else if (e.keyCode === 13) {
                document.getElementById("plato").value = data.Nombre;
                document.getElementById("precio").value = data.Pagar;
                OcultarCompletar(control.parentNode);
                control.parentNode.innerHTML = "";
            }
        });

        control.addEventListener("mouseover", function (e) {
            this.focus();
        });
    }

    //Calculos
    function pagar(data) {
        var sub = document.getElementById("subtotal");
        var desc = document.getElementById("descuento");
        var iva = document.getElementById("iva");
        var total = document.getElementById("total");

        var pagarDTO = {
            SubTotal: 0,
            Descuento: 0,
            IVA: 0,
            Total: 0
        };

        data.Platos.map((p) => {
            pagarDTO.SubTotal += (p.Precio * p.Cantidad);
            pagarDTO.Total += (p.Pagar * p.Cantidad);
            pagarDTO.Descuento = (pagarDTO.SubTotal - pagarDTO.Total);
        });

        if (document.getElementById("consumidor").checked === true) {
            sub.value = parseFloat(pagarDTO.SubTotal).toFixed(2);
            desc.value = parseFloat(pagarDTO.Descuento).toFixed(2);
            iva.value = pagarDTO.IVA;
            total.value = parseFloat(pagarDTO.Total).toFixed(2);
        }
        else {
            pagarDTO.SubTotal = parseFloat(pagarDTO.SubTotal / 1.13).toFixed(2);
            pagarDTO.IVA = parseFloat((pagarDTO.SubTotal) * 0.13).toFixed(2);
            sub.value = pagarDTO.SubTotal;
            desc.value = parseFloat(pagarDTO.Descuento).toFixed(2);
            iva.value = pagarDTO.IVA;
            total.value = parseFloat(pagarDTO.Total).toFixed(2);
        }

        return pagarDTO;
    }

    //Generar el contenido de la tabla
    function generarFilas(tabla, data) {
        tabla.innerHTML = "";
        var indice = 0;
        if (data.Platos !== null) {
            data.Platos.map((p) => {
                if (document.getElementById("consumidor").checked === true) {
                    tabla.innerHTML += '<tr><td>' + p.Nombre + '</td><td>$' + parseFloat(p.Precio).toFixed(2) + '</td><td><button type="button" id="menos-' + indice + '" class="password mr-1 btn btn-danger"><i class="fa fa-minus-square"></i></button> ' + p.Cantidad + ' <button type="button" id="mas-' + indice + '" class="password ml-1 btn btn-success"><i class="fa fa-plus-square"></i></button></td><td>$' + parseFloat(p.Precio * p.Cantidad).toFixed(2) + '</td></tr>';
                }
                else {
                    tabla.innerHTML += '<tr><td>' + p.Nombre + '</td><td>$' + parseFloat(p.Precio / 1.13).toFixed(2) + '</td><td><button type="button" id="menos-' + indice + '" class="password mr-1 btn btn-danger"><i class="fa fa-minus-square"></i></button> ' + p.Cantidad + ' <button type="button" id="mas-' + indice + '" class="password ml-1 btn btn-success"><i class="fa fa-plus-square"></i></button></td><td>$' + parseFloat((p.Precio * p.Cantidad) / 1.13).toFixed(2) + '</td></tr>';
                }                
                indice++;
            });
            if (data.Platos !== undefined && data.Platos.length > 0) {
                for (var i = 0; i < data.Platos.length; i++) {
                    restarPlato(document.getElementById("menos-" + i), data.Platos[i], data);
                    sumarPlato(document.getElementById("mas-" + i), data.Platos[i], data);
                }
            }
            pagar(data);
        }        
    }

    //Agregar plato a la mesa
    function agregarPlatoMesa(boton, data) {
        boton.addEventListener("click", function (e) {
            if (document.getElementById("plato").value.trim().length > 0 && document.getElementById("precio").value.trim().length > 0) {
                if (data.Platos !== null && data.Platos !== undefined) {
                    var registro = data.Platos.find((p) => p.Nombre === document.getElementById("plato").value.trim().toUpperCase());
                    if (registro !== undefined) {
                        registro.Cantidad += 1;
                        actualizarEstado(0);
                        generarFilas(document.getElementById("cuerpoTabla"), data);
                    }
                    else {
                        var plato = datos[0].Platos.find((p) => p.Nombre === document.getElementById("plato").value.trim().toUpperCase());
                        if (plato !== undefined) {
                            plato.Cantidad = 1;
                            data.Platos.push(plato);
                            actualizarEstado(0);
                            generarFilas(document.getElementById("cuerpoTabla"), data);
                        }
                        else {
                            MsgNoAgregado("El elemento que desea agregar no existe en el menú");
                        }
                    }
                }
                else {
                    data.Platos = [];
                    var plato = datos[0].Platos.find((p) => p.Nombre === document.getElementById("plato").value.trim().toUpperCase());
                    if (plato !== undefined) {
                        plato.Cantidad = 1;
                        data.Platos.push(plato);
                        actualizarEstado(0);
                        generarFilas(document.getElementById("cuerpoTabla"), data);
                    }
                    else {
                        MsgNoAgregado("El elemento que desea agregar no existe en el menú");
                    }
                }
            }
        });
    }

    //Restar plato de la mesa
    function restarPlato(boton, registro, data) {
        boton.addEventListener("click", function (e) {
            console.log(registro);
            if (registro.Cantidad > 1) {
                registro.Cantidad -= 1;
                actualizarEstado();
                generarFilas(document.getElementById("cuerpoTabla"), data);
            }
            else {
                var indice = data.Platos.find((p) => p.ID === registro.ID);
                data.Platos.splice(indice, 1);
                generarFilas(document.getElementById("cuerpoTabla"), data);
            }
        });
    }

    //Sumar plato de la mesa
    function sumarPlato(boton, registro, data) {
        boton.addEventListener("click", function (e) {
            registro.Cantidad += 1;
            actualizarEstado();
            generarFilas(document.getElementById("cuerpoTabla"), data);
        });
    }

    //Evento del botón facturar
    function facturar(boton, data) {
        boton.addEventListener("click", function (e) {
            if (data.Platos !== null && data.Platos !== undefined) {
                if (data.Platos.length > 0) {
                    var tipoFactura = {
                        TipoID: 0
                    };
                    if (document.getElementById("consumidor").checked === true) {
                        tipoFactura.TipoID = 1;
                    }
                    else {
                        tipoFactura.TipoID = 2;
                    }

                    var pagarDTO = pagar(data);

                    var facturaDTO = {
                        ID: 0,
                        TipoFacturaID: tipoFactura.TipoID,
                        EmpleadoID: 0,
                        ClienteID: data.Cliente.ID,
                        TipoPagoID: 1,
                        MesaID: data.ID,
                        Correlativo: "",
                        SubTotal: pagarDTO.SubTotal,
                        IVA: pagarDTO.IVA,
                        Descuento: pagarDTO.Descuento,
                        Total: pagarDTO.Total
                    };

                    $.ajax({
                        method: "POST",
                        url: "/factura/correlativo",
                        data: tipoFactura,
                        cache: false,
                        success: function (rst) {
                            if (rst !== "ERROR") {
                                facturaDTO.Correlativo = rst
                                var detalleDTO = [];

                                var precio = 0;

                                data.Platos.map((item) => {
                                    if (document.getElementById("consumidor").checked === true) {
                                        precio = item.Precio;
                                    }
                                    else {
                                        precio = (item.Precio / 1.13);
                                    }

                                    detalleDTO.push({
                                        MenuID: item.ID,
                                        Descripcion: item.Nombre,
                                        Precio: precio,
                                        Cantidad: item.Cantidad,
                                        Importe: (precio * item.Cantidad)
                                    });
                                });

                                detalles = {
                                    FacturaDTO: facturaDTO,
                                    DatosDetalle: JSON.stringify(detalleDTO)
                                };

                                $.ajax({
                                    method: "POST",
                                    url: "/factura/facturar",
                                    data: detalles,
                                    cache: false,
                                    success: function (resultado) {
                                        if (resultado !== "Error") {
                                            $.ajax({
                                                method: "POST",
                                                url: "/factura/generarfactura",
                                                data: { ID: parseInt(resultado) },
                                                cache: false,
                                                beforeSend: function () {
                                                    quitar(data);
                                                    PantallaDeCarga();
                                                },
                                                success: function (rst) {
                                                    EliminarPantallaCarga();                                                    
                                                    if (rst !== "ERROR") {
                                                        document.getElementById("docFactura").setAttribute("src", rst);
                                                        $("#verFactura").modal("show");                                                        
                                                    }
                                                    else {
                                                        MsgNoAgregado("Error al genera la factura");
                                                    }
                                                }
                                            });
                                        }
                                        else {
                                            MsgNoAgregado(resultado);
                                        }
                                    }
                                });
                            }
                            else {
                                MsgNoAgregado("Error al intentar generar el correlativo de la factura");
                            }
                        }
                    });
                }
                else {
                    MsgNoAgregado("Error al facturar. No se han agregado elementos a la mesa");
                }
            }
            else {
                MsgNoAgregado("Error al facturar. No se han agregado elementos a la mesa");
            }            
        });
    }
});
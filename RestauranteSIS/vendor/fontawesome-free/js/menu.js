$(document).ready(function () {
    var nombreControl = document.getElementById("nombre");
    var precioControl = document.getElementById("precio");
    var descuentoControl = document.getElementById("descuento");

    var datos;

    //Cargar datos

    function cargarDatos(registros) {
        datos = registros;
        var tabla = $("#tablaMenu").DataTable();
        tabla.destroy();
        var filas = document.getElementById("cTablaMenu");
        filas.innerHTML = "";
        var indice = 0;

        CargarCategoria(document.getElementById("categoria"), datos[0].Categorias);

        datos[0].ListaMenu.map((item) => {
            filas.innerHTML += '<tr><td>' + item.Nombre + '</td><td>$' + item.Precio + '</td><td>' + item.Descuento + '%</td><td>$' + item.Pagar + '</td><td>' + item.CategoriaNombre + '</td><td>' + ((item.Estado === 1) ? "Visible" : "Oculto") + '</td><td><buttom id="editMenu-' + indice + '" class="btn btn-primary m-1" data-toggle="modal" data-target=".editar-menu">Editar</buttom><buttom id="elimMenu-' + indice + '" class="btn btn-danger m-1">Eliminar</buttom></td></tr>';
            indice++;
        });

        for (var i = 0; i < datos[0].ListaMenu.length; i++) {
            Eliminar(document.getElementById("elimMenu-" + i), datos[0].ListaMenu[i]);
            EditarRegistro(document.getElementById("editMenu-" + i), datos[0].ListaMenu[i]);
        }

        tabla = $("#tablaMenu").DataTable();
    }

    //Cargar Datos Combobox
    function CargarCategoria(control, registos) {
        control.innerHTML = "";
        registos.map((item) => {
            control.innerHTML += '<option value="' + item.ID + '">' + item.Nombre + '</option>';
        });
    }

    //Evento Eliminar
    function Eliminar(boton, dataEliminar) {
        boton.addEventListener("click", function (e) {
            AlertaBorrar(dataEliminar.ID, "/menu/quitar", function (Rst) {
                if (Rst === true) {
                    $.ajax({
                        method: "POST",
                        url: "/menu/quitarimagen",
                        data: { Ruta: dataEliminar.Imagen },
                        cache: false,
                        success: function (Resultado) {
                            if (Resultado === "OK") {
                                MsgAgregado("La imágen fué removida del servidor");
                                recuperarDatos();
                            }
                            else {
                                MsgNoAgregado("No se pudo eliminar la imágen del servidor");
                            }
                        }
                    });
                }
            });
        });
    }

    //Recuperar datos

    function recuperarDatos() {
        $.ajax({
            method: "GET",
            url: "/datos/mostrarrelcategoriamenu",
            Cache: false,
            processData: false,
            dataType: "JSON",
            success: function (resultado) {
                cargarDatos(resultado);
            }
        });
    }
    recuperarDatos();    
    LimitarTexto(nombreControl, 35, true);
    Moneda(precioControl);
    DescuentoValidar(descuentoControl);
    CargarImagen("archivo", "visualizar");

    function DescuentoValidar(control) {
        control.addEventListener("keypress", function (e) {
            var expresion = /^[0-9]+$/;
            if (!expresion.test(e.key) || parseInt(control.value + e.key) >= 100) {
                e.preventDefault();
            }
        });
    }

    //Subir imagen
    function SubirImagen(imagen, nombre, ruta) {
        var nombreArchivo = "";

        for (var i = 0; i < nombre.length; i++) {
            if (nombre.substr(i, 1) === " ") {
                nombreArchivo += "_";
            }
            else {
                nombreArchivo += nombre.substr(i, 1);
            }
        }

        var formulario = new FormData();
        formulario.append("Nombre", nombreArchivo);
        formulario.append("Archivo", imagen.files[0]);
        $.ajax({
            method: "POST",
            url: "/menu/subirimagen",
            data: formulario,
            cache: false,
            processData: false,
            contentType: false,
            success: function (resultado) {
                ruta(resultado);
            }
        });
    }

    //Agregar nuevo elemento al menú
    document.getElementById("guardar").addEventListener("click", function (e) {
        var nombre, descripcion, imagen;
        var descuento, estado, categoriaID;
        var precio;

        var alertas = document.getElementById("alertas");
        var imagenControl = document.getElementById("archivo");

        alertas.innerHTML = "";

        estado = document.getElementById("estado").value;

        if (TieneContenido(nombreControl.value.trim()) && nombreControl.value.trim().length <= 35) {
            if (datos !== undefined) {
                var buscarNombre = datos[0].ListaMenu.find((item) => item.Nombre.toUpperCase() === nombreControl.value.trim().toUpperCase());
                nombre = (buscarNombre === undefined) ? nombreControl.value.trim().toUpperCase() : undefined;
                if (nombre === undefined) {
                    MensajeError(alertas, "Ya hay un registro con ese nombre");
                }
            }
            else {
                nombre = nombreControl.value.trim().toUpperCase();
            }
        }
        else {
            MensajeError(alertas, "Error en nombre");
        }

        if (TieneContenido(precioControl.value.trim()) && EsDecimal(precioControl.value.trim())) {
            if (parseFloat(precioControl.value.trim()) > 0) {
                precio = parseFloat(precioControl.value.trim());
            }
            else {
                MensajeError(alertas, "El precio debe ser mayor a cero");
            }
        }
        else {
            MensajeError(alertas, "Hay un error en el precio");
        }

        if (TieneContenido(descuentoControl.value.trim())) {
            if (parseInt(descuentoControl.value.trim()) >= 100) {
                MensajeError(alertas, "Error en el porcentaje de descuento");
            }
            else {
                descuento = parseInt(descuentoControl.value.trim());
            }
        }
        else {
            descuento = 0;
        }

        if (TieneContenido(document.getElementById("descripcion").value.trim())) {
            descripcion = document.getElementById("descripcion").value.trim();
        }
        else {
            MensajeError(alertas, "Agregar una descripción");
        }

        if (imagenControl.files[0] !== undefined) {
            imagen = imagenControl.value;
        }
        else {
            MensajeError(alertas, "Elegir una imagen");
        }

        if (document.getElementById("categoria").value !== undefined) {
            categoriaID = document.getElementById("categoria").value;
        }

        if (nombre !== undefined && precio !== undefined && descuento !== undefined && descripcion !== undefined && imagen !== undefined && categoriaID !== undefined) {
            SubirImagen(imagenControl, nombre, function (Resultado) {
                if (Resultado !== "ERROR") {
                    var datosDTO = {
                        Nombre: nombre,
                        Precio: precio,
                        Descuento: descuento,
                        Descripcion: descripcion,
                        Imagen: Resultado,
                        Estado: estado,
                        CategoriaID: categoriaID
                    };

                    $.ajax({
                        method: "POST",
                        url: "/menu/agregar",
                        data: datosDTO,
                        cache: false,
                        success: function (Rst) {
                            if (Rst === "OK") {
                                MsgAgregado("Se ha agregado un nuevo elemento en el menú");
                                LimpiarControl(document.getElementById("descripcion"));
                                LimpiarTextBox(".textbox", document.getElementById("guardar"));
                                OcultarImagen("archivo", imagenControl.files[0], document.getElementById("visualizar"));
                                OcultarVentana(document.getElementById("agregarMenu"));
                                recuperarDatos();
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

    //Editar elemento del menú
    function EditarRegistro(boton, dataEditar) {
        boton.addEventListener("click", function (e) {
            var formEdit = document.getElementById("formEdit");
            formEdit.innerHTML = '<div class="modal-header"><h3 class="modal-title" id="myLargeModalLabel">DATOS ELEMENTO DEL MENÚ</h3><button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">×</span></button></div><div class="grupoControles"><div class="controles"><label for="nombreEdit">Nombre:</label><input class="usuario control" type="text" name="nombreEdit" id="nombreEdit" value="' + dataEditar.Nombre + '" readonly /></div><div class="controles"><input type="text" name="precio" class="textbox" id="precioEdit" value="' + dataEditar.Precio + '"><div class="texto noplaceholder">Precio</div></div></div><div class="grupoControles"><div class="controles"><input type="text" name="descuento" class="textbox" id="descuentoEdit" value="' + dataEditar.Descuento + '"><div class="texto noplaceholder">Descuento</div></div><div class="controles"><div class="textoarea  noplaceholderareas">Descripción</div><textarea class="area" name="descripcion" id="descripcionEdit">' + dataEditar.Descripcion + '</textarea></div></div><div class="grupoControles"><div class="controles"><input type="file" name="imagen" id="archivoEdit"><label for="archivoEdit" class="archivo">Seleccionar imágen</label><article id="visualizarEdit" class="ocultarArchivo"></article></div><div class="controles"><label for="categoria">Categoría:</label><select name="categoria" class="control" id="categoriaEdit"></select></div></div><div class="grupoControles"><div class="controles"><label for="estado">Mostrar:</label><select name="estado" class="control" id="estadoEdit"><option value="1">SI</option><option value="0">NO</option></select></div></div><div class="grupoControles"><button id="editar" class="btnEnviar" type="button">Editar</button></div><div id="alertasEdit"></div>';
            EventosControles();

            CargarImagen("archivoEdit", "visualizarEdit");

            var vista = document.getElementById("visualizarEdit");

            vista.setAttribute("style", "background-image: url(" + dataEditar.Imagen + ");");
            vista.className = "mostrarArchivo";

            CargarCategoria(document.getElementById("categoriaEdit"), datos[0].Categorias);

            document.getElementById("categoriaEdit").value = dataEditar.CategoriaID;

            document.getElementById("estadoEdit").value = dataEditar.Estado;

            GuardarEdicion(document.getElementById("editar"), dataEditar);
        });
    }

    function GuardarEdicion(boton, dataEditar) {
        boton.addEventListener("click", function (e) {
            var descripcion;
            var descuento, estado, categoriaID;
            var precio;

            var descripControl = document.getElementById("descripcionEdit");
            var descuentoControl = document.getElementById("descuentoEdit");
            var estadoControl = document.getElementById("estadoEdit");
            var categoriaControl = document.getElementById("categoriaEdit");
            var precioControl = document.getElementById("precioEdit");
            var alertas = document.getElementById("alertasEdit");
            var imagenControl = document.getElementById("archivoEdit");

            alertas.innerHTML = "";

            estado = parseInt(estadoControl.value);

            if (TieneContenido(precioControl.value.trim()) && EsDecimal(precioControl.value.trim())) {
                if (parseFloat(precioControl.value.trim()) > 0) {
                    precio = parseFloat(precioControl.value.trim());
                }
                else {
                    MensajeError(alertas, "El precio debe ser mayor a cero");
                }
            }
            else {
                MensajeError(alertas, "Error en el precio");
            }

            if (TieneContenido(descuentoControl.value.trim())) {
                if (parseFloat(descuentoControl.value.trim()) >= 0 && parseFloat(descuentoControl.value.trim()) < 100) {
                    descuento = parseFloat(descuentoControl.value.trim());
                }
                else {
                    MensajeError(alertas, "Error en el descuento");
                }
            }
            else {
                descuento = 0;
            }

            if (TieneContenido(descripControl.value.trim())) {
                descripcion = descripControl.value.trim();
            }
            else {
                MensajeError(alertas, "Agregar descripción");
            }

            if (TieneContenido(categoriaControl.value) && parseInt(categoriaControl.value) > 0) {
                categoriaID = parseInt(categoriaControl.value);
            }

            if (precio !== undefined && descuento !== undefined && descripcion !== undefined && estado !== undefined && categoriaID !== undefined) {
                if (imagenControl.files[0] !== undefined) {
                    SubirImagen(imagenControl, dataEditar.Nombre, function (rst) { });
                }
                var editarDTO = {
                    ID: dataEditar.ID,
                    Nombre: dataEditar.Nombre,
                    Precio: precio,
                    Descuento: descuento,
                    Descripcion: descripcion,
                    Imagen: dataEditar.Imagen,
                    Estado: estado,
                    CategoriaID: categoriaID
                };

                $.ajax({
                    method: "POST",
                    url: "/menu/editar",
                    data: editarDTO,
                    cache: false,
                    success: function (Rst) {
                        if (Rst === "OK") {
                            MsgAgregado("Se han actualizado los datos en el elemento del menú");
                            OcultarVentana(document.getElementById("editarMenu"));
                            recuperarDatos();
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
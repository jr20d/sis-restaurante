$(document).ready(function (e) {
    CargarImagen("archivo", "visualizar");
    var nombreControl = document.getElementById("nombre");
    LimitarTexto(nombreControl, 15, true);
    EventosControles();
    var contenedor = document.getElementById("contenedor-categoria");

    var datos;

    function cargarDatos(registros) {
        datos = registros;

        contenedor.innerHTML = "";

        var indice = 0;

        datos.map((item) => {
            contenedor.innerHTML += '<div class="card text-white bg-dark m-3" style="max-width: 30%; min-width: 250px;"><div class="card-header text-center"> <h5 class="card-title">' + item.Nombre + '</h5></div><img class="card-img opaco" id="img-1" src="' + item.Imagen + '" alt="Card image"><div id="content-1" style="position: absolute; top: 50%; width: 100%;"><button id="editar-' + indice + '" type="button" style="cursor:pointer;" class="d-block mx-auto btn btn-outline-dark w-50" data-toggle="modal" data-target=".editar-categoria">Editar</button></div></div>';
            indice++;
        });

        for (var i = 0; i < datos.length; i++) {
            Editar(document.getElementById("editar-" + i), datos[i]);
        }
    }

    //Evento editar
    function Editar(boton, dataEditar) {
        var formularioEditar = document.getElementById("formEdit");
        boton.addEventListener("click", function (e) {
            formularioEditar.innerHTML = '<div class="modal-header"><h3 class="modal-title" id="myLargeModalLabel"> DATOS CATEGORIA</h3><button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">×</span></button></div><div class="grupoControles"><div class="controles"><label for="nombreEdit">Nombre categoria:</label><input type="text" name="nombreEdit" class="usuario control" id="nombreEdit" value="' + dataEditar.Nombre + '" readonly></div><div class="controles"><input type="file" name="imagen" id="archivoEdit" value="' + dataEditar.Imagen + '"><label for="archivoEdit" class="archivo">Seleccionar imágen</label><article id="visualizarEdit" class="ocultarArchivo"></article></div></div><div class="grupoControles"><button id="editar" class="btnEnviar" type="button">Guardar</button></div><div id="alertasEdit"></div>';          

            var vista = document.getElementById("visualizarEdit");

            vista.setAttribute("style", "background-image: url(" + dataEditar.Imagen + ");");
            vista.className = "mostrarArchivo";

            CargarImagen("archivoEdit", "visualizarEdit");

            document.getElementById("archivoEdit").setAttribute("value", dataEditar.Imagen);

            editarDatos(dataEditar);
        });
    }

    //Actualizar datos
    function editarDatos(dataEditar) {
        document.getElementById("editar").addEventListener("click", function (e) {
            var alertas = document.getElementById("alertasEdit");
            var archivoEdit = document.getElementById("archivoEdit");
            alertas.innerHTML = "";

            console.log(archivoEdit.getAttribute("value"));

            if (archivoEdit.files[0] !== undefined) {
                var formEdit = new FormData();
                formEdit.append("Nombre", dataEditar.Nombre);
                formEdit.append("Archivo", archivoEdit.files[0]);

                $.ajax({
                    method: "POST",
                    url: "/menu/subirimagen",
                    data: formEdit,
                    cache: false,
                    processData: false,
                    contentType: false,
                    success: function (Resultado) {
                        if (Resultado !== "ERROR") {
                            MsgAgregado("Los datos de la categoría se han actualizado con éxito");
                            OcultarVentana(document.getElementById("editarCategoria"));
                            RecuperarDatos();
                        }
                        else {
                            MsgNoAgregado("Error al actualizar los datos de la categoría");
                        }
                    }
                });
            }
            else {
                MensajeError(alertas, "No ha seleccionado una imágen o está intentado guardar la misma imágen");
            }
        });
    }

    //Recuperar datos
    function RecuperarDatos() {
        $.ajax({
            method: "GET",
            url: "/datos/mostrarcategorias",
            Cache: false,
            processData: false,
            dataType: "JSON",
            success: function (resultado) {
                cargarDatos(resultado);
            }
        });
    }

    RecuperarDatos();

    document.getElementById("guardar").addEventListener("click", function (e) {
        var nombre, imagen;
        var archivo = document.getElementById("archivo");
        var alertas = document.getElementById("alertas");

        alertas.innerHTML = "";

        if (TieneContenido(nombreControl.value.trim()) && nombreControl.value.trim().length <= 15) {
            if (datos !== undefined) {
                var buscarNombre = datos.find((item) => item.Nombre.toUpperCase() === nombreControl.value.trim().toUpperCase());
                nombre = (buscarNombre === undefined) ? nombreControl.value.trim().toUpperCase() : undefined;
                if (nombre === undefined) {
                    MensajeError(alertas, "El nombre de la categoría ya está registrada");
                }
            }
            else {
                nombre = nombreControl.value.trim().toUpperCase();
            }
        }
        else {
            MensajeError(alertas, "Error en el nombre de la categoría");
        }

        if (TieneContenido(archivo.value)) {
            imagen = archivo.value;
        }
        else {
            MensajeError(alertas, "Agregar una imágen");
        }

        if (nombre !== undefined && imagen !== undefined) {
            var formulario = new FormData();
            formulario.append("Nombre", nombre);
            formulario.append("Archivo", archivo.files[0]);

            var categoriaDTO = {
                Nombre: nombre,
                Imagen: imagen
            };

            $.ajax({
                method: "POST",
                url: "/menu/subirimagen",
                data: formulario,
                cache: false,
                processData: false,
                contentType: false,
                success: function (Resultado) {
                    if (Resultado !== "ERROR") {
                        categoriaDTO.Imagen = Resultado;
                        $.ajax({
                            method: "POST",
                            url: "/menu/agregarcategoria",
                            data: categoriaDTO,
                            Cache: false,
                            success: function (Resultado) {
                                if (Resultado === "OK") {
                                    MsgAgregado("La categoría ha sido agregada con éxito");
                                    LimpiarTextBox(".textbox", document.getElementById("guardar"));
                                    OcultarImagen("archivo", archivo.files[0], document.getElementById("visualizar"));
                                    OcultarVentana(document.getElementById("agregarCategoria"));
                                    RecuperarDatos();
                                }
                                else {
                                    MsgNoAgregado(Resultado);
                                }
                            }
                        });
                    }
                    else {
                        MsgNoAgregado("Error al subir la imágen de la categoría");
                    }
                }
            });
        }
    });
});
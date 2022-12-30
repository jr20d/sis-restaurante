$(document).ready(function () {
    EventosControles();
});

function EventosControles() {
    var input = $(".textbox");
    var placeholder = $(".texto");
    input.on("focus", function (e) {
        var texto = this.parentNode.children[1];
        texto.className = "texto noplaceholder";
    });
    input.on("focusout", function (e) {
        if (this.value.trim().length == 0) {
            var texto = this.parentNode.children[1];
            texto.className = "texto placeholder";
        }
    });
    placeholder.on("click", function (e) {
        this.className = "texto noplaceholder";
        var control = this.parentNode.children[0];
        control.focus();
    });

    var textoarea = $(".textoarea");
    var area = $(".area");

    area.on("focus", function (e) {
        var texto = this.parentNode.children[0];
        texto.className = "textoarea noplaceholderareas";
    });
    area.on("click", function (e) {
        var texto = this.parentNode.children[0];
        texto.className = "textoarea noplaceholderareas";
    });
    area.on("focusout", function (e) {
        if (this.value.trim().length == 0) {
            var texto = this.parentNode.children[0];
            texto.className = "textoarea placeholderareas";
        }
    });
    textoarea.on("click", function (e) {
        this.className = "textoarea noplaceholderareas";
        var control = this.parentNode.children[1];
        control.focus();
    });
    $("#telefono").on("keypress", function (e) {
        var expReg = /^[0-9]+$/;
        if (expReg.test(e.key) == false || $("#telefono").val().length >= 9) {
            e.preventDefault();
        }
        if ($("#telefono").val().length === 4) {
            var tel = document.getElementById("telefono");
            tel.value += "-";
        }
    });
    $("#telefonoEdit").on("keypress", function (e) {
        var expReg = /^[0-9]+$/;
        if (expReg.test(e.key) == false || $("#telefonoEdit").val().length >= 9) {
            e.preventDefault();
        }
        if ($("#telefonoEdit").val().length === 4) {
            var tel = document.getElementById("telefonoEdit");
            tel.value += "-";
        }
    });
    $("#dui").on("keypress", function (e) {
        var expReg = /^[0-9]+$/;
        if (expReg.test(e.key) == false || $("#dui").val().length >= 10) {
            e.preventDefault();
        }
        if ($("#dui").val().length === 8) {
            var tel = document.getElementById("dui");
            tel.value += "-";
        }
    });
        
    $("#nit").on("keypress", function (e) {
        expReg = /[0-9]+$/;
        if (!expReg.test(e.key) || $("#nit").val().length === 17) {
            e.preventDefault();
        }
        if ($("#nit").val().length === 4 || $("#nit").val().length === 11 || $("#nit").val().length === 15) {
            var nit = document.getElementById("nit");
            nit.value += "-";
        }
    });
    function VerPassword() {
        var icono = document.getElementById("iconoPassword");
        icono.className = (icono.className === "fa fa-eye") ? "fa fa-eye-slash" : "fa fa-eye";
        icono.parentNode.parentNode.children[0].type = (icono.className === "fa fa-eye") ? "password" : "text";
    }
    $(".password").on("click", VerPassword);
}

function LimitarTexto(control, caracteres, soloTexto) {
    control.addEventListener("keypress", function (e) {
        var expresion;
        if (soloTexto === true) {
            expresion = /^[a-zA-ZáéíóúÁÉÍÓÚñÑ ]+$/;
        }
        else {
            expresion = /^[a-zA-ZáéíóúÁÉÍÓÚ0-9 ]+$/;
        }

        if (!expresion.test(e.key) || control.value.length >= caracteres) {
            e.preventDefault();
        }
        else if (control.value === "" && e.key === " ") {
            e.preventDefault();
        }
    });
}

function SoloEnteros(control) {
    control.addEventListener("keypress", function (e) {
        var expresion = /^[0-9]+$/;
        if (!expresion.test(e.key)) {
            e.preventDefault();
        }
    });
}

function Password(control, caracteres) {
    control.addEventListener("keypress", function (e) {
        var expresion = /^[ ]+$/;
        if (expresion.test(e.key) === true || control.value.length >= caracteres) {
            e.preventDefault();
        }
    });
}

function HayPuntoDecimal(cadena) {
    for (var i = 0; i <= cadena.length; i++) {
        if (cadena.substr(i, 1) === ".") {
            return true;
        }
    }
    return false;
}

function Moneda(control) {
    control.addEventListener("keypress", function (e) {
        var expresion = /^[0-9.]+$/;
        if (!expresion.test(e.key) || (HayPuntoDecimal(control.value) === true && e.key === ".")) {
            e.preventDefault();
        }
        else if (control.value === "" && e.key === ".") {
            salario.value = "0";
        }
    });
}

function MensajeError(contenedor, mensaje) {
    contenedor.innerHTML += "<p class='alert alert-danger text-center my-1'>" + mensaje + "</p>";
}

function LimpiarControl(control) {
    control.value = "";
}

function TieneContenido(texto) {
    if (texto.trim().length === 0) {
        return false;
    }
    else {
        return true;
    }
}

function EsDecimal(valor) {
    if (!parseFloat(valor)) {
        return false;
    }
    else {
        return true;
    }
}

function MsgAgregado(mensaje) {
    Swal.fire({
        icon: 'success',
        title: 'CORRECTO',
        text: mensaje
    });
}

function OcultarVentana(ventana) {
    $(ventana).modal("hide");
}

function MsgNoAgregado(mensaje) {
    Swal.fire({
        icon: 'error',
        title: 'ERROR',
        text: mensaje
    });
}

function LimpiarTextBox(claseControles, boton) {
    for (var i = 0; i < $(claseControles).length; i++) {
        $(claseControles)[i].value = "";
        $(claseControles)[i].focus();
    }
    boton.focus();
}

function AlertaBorrar(id, urlEliminar, retorno) {
    if (id != undefined) {
        Swal.fire({
            icon: "warning",
            title: "BORRAR REGISTRO",
            text: "¿Está seguro de querer realizar esta acción?",
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Borrar',
            cancelButtonText: "Cancelar"
        }).then((resultado) => {
            if (resultado.value) {
                var DataDTO = {ID: id}
                $.ajax({
                    method: "POST",
                    url: urlEliminar,
                    data: DataDTO,
                    cache: false,
                    success: function (resultado) {
                        if (resultado === "OK") {
                            MsgAgregado("El registro se removió con éxito");
                            retorno(true);
                        }
                        else {
                            MsgNoAgregado(resultado);
                            retorno(false);
                        }
                    }
                });
            }
        });
    }
}

function ValidarTexto(texto) {
    var expresion = /^[0-9-.,^"'!|()=&%$#/¿?{}]+$/;
    if (expresion.test(texto)) {
        return false;
    }
    else {
        return true
    }
}

function OcultarImagen(idControl, img, ver) {
    ver.className = "ocultarArchivo";
    img = null;
    $("#" + idControl + "").val(undefined);
}

function VisualizarReporte(control, ruta, datos, contenedor, idModal) {
    control.addEventListener("click", function (e) {
        $.ajax({
            method: "POST",
            url: ruta,
            data: datos,
            cache: false,
            beforeSend: PantallaDeCarga(),
            success: function (Resultado) {
                EliminarPantallaCarga();
                if (Resultado !== null || Resultado !== undefined) {
                    contenedor.setAttribute("src", Resultado);
                    $("#" + idModal).modal("show");
                }
            }
        });
    });
}

function PantallaDeCarga() {
    var carga = document.createElement("div");
    carga.setAttribute("id", "carga");
    carga.setAttribute("class", "pantalla");
    carga.innerHTML = '<div class="loader"><span class="cuadros"></span><span class="cuadros"></span><span class="cuadros"></span><span class="cuadros"></span></div>';
    var cuerpo = document.getElementsByTagName("body")[0];
    cuerpo.appendChild(carga);
}

function EliminarPantallaCarga() {
    var carga = document.getElementById("carga");
    var cuerpo = document.getElementsByTagName("body")[0];
    cuerpo.removeChild(carga);
}

function BuscarNombreUsuario(usuario) {
    var userName = "";
    var expresion = /^[a-zA-Z.]+$/;

    for (var i = 0; i < usuario.trim().length; i++) {
        if (expresion.test(usuario.substr(i, 1))) {
            userName += usuario.substr(i, 1);
        }
    }
    return userName;
}

function CargarImagen(idImagen, idVer) {
    $("#" + idImagen).on("change", function (e) {
        try {
            var img = e.target.files[0];
            var ver = document.getElementById(idVer);
            if (img.type === "image/jpeg" || img.type === "image/png") {
                var verImagen = new FileReader();
                verImagen.onload = function () {
                    var url = verImagen.result;
                    ver.setAttribute("style", "background-image: url(" + url + ");");
                    ver.className = "mostrarArchivo";
                }
                verImagen.readAsDataURL(img);
            }
            else {
                OcultarImagen("archivo", img, ver);
                e.preventDefault();
                return false;
            }
        } catch (e) {
            OcultarImagen("archivo", img, ver);
            e.preventDefault();
            return false;
        }
    });
}

//Validación de NIT
function ValidarFechaNIT(cadena) {
    var dia = cadena.substr(5, 2);
    var mes = cadena.substr(7, 2);
    var year = 20 + cadena.substr(9, 2);
    var fecha = year + "-" + mes + "-" + dia;
    var validar;

    if (Date.parse(fecha)) {
        validar = true
    }
    else {
        validar = false;
    }
    return validar;
}
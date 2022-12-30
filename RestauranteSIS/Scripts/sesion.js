$(document).ready(function () {
    document.getElementById("sesion").addEventListener("click", function (e) {
        var usuarioControl = document.getElementById("usuario");
        var passControl = document.getElementById("pass");

        if (usuarioControl.value.trim().length > 0 && passControl.value.trim().length > 0) {
            var sesionDTO = {
                Usuario: usuarioControl.value.trim(),
                Password: passControl.value.trim()
            };

            $.ajax({
                method: "POST",
                url: "/sesion/iniciar",
                data: sesionDTO,
                cache: false,
                success: function (rst) {
                    if (rst !== "OK") {
                        MsgNoAgregado(rst);
                    }
                    else {
                        location.href = "/principal/";
                    }
                }
            });
        }
        else {
            MsgNoAgregado("Asegurese de ingresar el nombre de usuario y contraseña");
        }
    });
});
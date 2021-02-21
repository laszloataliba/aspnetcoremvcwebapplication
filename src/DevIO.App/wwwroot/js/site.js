function AjaxModal() {

    $(document).ready(function () {
        $(function () {
            $.ajaxSetup({ cache: false });

            $('a[data-modal]').on('click', function (e) {
                $('#myModalContent').load(this.href,
                    function () {
                        $('#myModal').modal({
                            keyboard: true
                        }, 'show');

                        bindForm(this);
                    }
                );

                return false;
            });
        });
    });

    function bindForm(dialog) {
        $('form', dialog).submit(function () {
            $.ajax({
                url: this.action,
                type: this.method,
                data: $(this).serialize(),
                success: function (result) {
                    if (result.success) {
                        $('#myModal').modal('hide');
                        $('#AddressTarget').load(result.url);
                    }
                    else {
                        $('#myModalContent').html(result);
                        bindForm(dialog);
                    }
                }
            });

            return false;
        });
    }
}

function BuscaCep() {
    $(document).ready(function () {

        function LimpaFormularioCep() {
            $('#Address_StreetAddress').val('');
            $('#Address_Neighborhood').val('');
            $('#Address_City').val('');
            $('#Address_State').val('');
        }

        $('#Address_ZipCode').blur(function () {
            var cep = $(this).val().replace(/\D/g, '');

            if (cep != '') {
                var validaCep = /^[0-9]{8}$/;

                if (validaCep.test(cep)) {
                    $('#Address_StreetAddress').val('...');
                    $('#Address_Neighborhood').val('...');
                    $('#Address_City').val('...');
                    $('#Address_State').val('...');

                    $.getJSON(`https://viacep.com.br/ws/${cep}/json/?callback=?`,
                        function (dados) {
                            if (!('erro' in dados)) {
                                $('#Address_StreetAddress').val(dados.logradouro);
                                $('#Address_Neighborhood').val(dados.bairro);
                                $('#Address_City').val(dados.localidade);
                                $('#Address_State').val(dados.uf);
                            }
                            else {
                                LimpaFormularioCep();
                                alert('CEP não encontrado.');
                            }
                        }
                    );
                }
                else {
                    LimpaFormularioCep();
                    alert('Formato de CEP inválido.');
                }
            }
            else {
                LimpaFormularioCep();
            }
        });
    });
}

$(document).ready(function () {
    $('#msg_box').fadeOut(2500);
});
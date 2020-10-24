$(document).ready(function () {
    $('#ServiceId').select2({ width: '100%' });
});
CKEDITOR.replace('LongContent',
    {
        customConfig: '/lib/ckeditor/Config.js'
    });
CKEDITOR.replace('Description',
    {
        customConfig: '/lib/ckeditor/Config.js'
    });

/*function submit() {
    if ($('#create-form').valid() == true) {
        $('#create-form').submit();
    }
    if ($('#ParentId').val() >= 0) {
        if ($('#edit-form').valid() == true) {
            $('#edit-form').submit();
        }
    } else {
        UIkit.notification({ message: 'لطفا دسته والد سرویس را انتخاب کنید.', status: 'danger', pos: 'bottom-center', timeout: 5000 });
    }
}*/

function readURL(input) {
    if (input.files && input.files[0]) {

        var reader = new FileReader();
        reader.onload = function (e) {
            $('#imgSample').attr('src', e.target.result);
        }
        reader.readAsDataURL(input.files[0]); // convert to base64 string
    }
}

$("#sampleFile").change(function () {
    readURL(this);
});

$('#Title').blur(function () {
    let input = document.getElementById('Title');
    checkDupTitle(input);
});
function checkDupTitle(title) {
    var titleInput = $(title);
    var workSampleTitle = titleInput.val();
    var titleValidate = $('#titleValidate').html('');
    if (titleInput.hasClass('uk-form-success') || titleInput.hasClass('uk-form-danger')) {
        titleInput.removeClass('uk-form-success');
        titleInput.removeClass('uk-form-danger');
    }
    titleValidate.hide();
    $.post('/admin/check-duplicate-title', { 'workSampleTitle': workSampleTitle },
        function (response) {
            if (response === true) {
                var status = "danger", msg = "عنوان نمونه‌کار نمیتواند تکراری باشد";
                UIkit.notification({ message: msg, status: status, pos: 'bottom-center', timeout: 5000 });
                titleValidate.html(msg).show();
                titleValidate.addClass('uk-text-danger');
                titleInput.addClass('uk-form-danger');
            } else {
                titleValidate.addClass('uk-text-success').show();
                titleInput.addClass('uk-form-success');
                titleInput.find('#titleLabel').addClass('uk-text-success');
            }
        }, 'json').then(function () {
            setTimeout(function () {
                if (input.hasClass('uk-form-success') || input.hasClass('uk-form-danger')) {
                    input.removeClass('uk-form-success');
                    input.removeClass('uk-form-danger');
                }
            }, 2000);
        });
}
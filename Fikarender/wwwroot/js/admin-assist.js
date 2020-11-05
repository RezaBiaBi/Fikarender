function filterSwitch(tag) {
    let value = $(tag).val();
    let html = '';
    switch (value) {
        case 'fullname':
            html = '<label class="uk-form-label">نام و نام خانوادگی را تایپ کنید</label><input type="text" name="param" class="uk-input uk-border-rounded" />'
            break;
        case 'phone':
            html = '<label class="uk-form-label">تلفن همراه را تایپ کنید</label><input type="number" name="param" class="uk-input uk-border-rounded" />'
            break;
        default:
            break;
    }
    if (value != 'tag') {
        $('#append-filter').html(html);
    }
}
function filter() {
    let filterBy = $('select[name=filterBy]').val();
    if (filterBy != 0) {
        preloader();
        let data = $('#search-modal form').serialize();
        $.post('/admin/assist', data, function (r) {
            $('#append').html(r);
        }, 'html').then(function () {
            UIkit.modal('#search-modal').hide();
            preloader();
        });
    } else {
        UIkit.notification({ message: 'لطفا نحوه فیلتر را انتخاب و مقدار آن را پر کنید.', status: 'danger', pos: 'bottom-center', timeout: 6000 });
        $('select[name=filterBy]').focus();
    }
}
$('#search-modal form').on('submit', function (e) {
    e.preventDefault();
    filter();
});
function downloadCvFile() {
    var assistId = $('#download-form input[name = "assistId"]').val();
    $('#preloader').fadeIn();
    $('#download-form').submit();
    $.post('/admin/download-cv-file', { 'assistId': assistId },
        function (response) {
            if (response === false) {
                UIkit.notification({ message: response.msg, status: response.status, pos: 'bottom-center', timeout: 5000 });
            } else {
                /*UIkit.notification({ message: response.msg, status: response.status, pos: 'bottom-center', timeout: 5000 });*/
            }
        }, 'json').then(function () {
            $('#preloader').fadeOut();
        });
}
function showAssist(assistId) {
    /*event.stopPropagation();*/
    preloader();
    $.get('/admin/assist', { 'assistId': assistId },
        function (response) {
            $('#show-assist-modal').html(response);
        }, 'html').then(function () {
            setTimeout(function () {
                UIkit.modal('#show-assist-modal').show();
                $('#preloader').hide();
            }, 500);
        });
}
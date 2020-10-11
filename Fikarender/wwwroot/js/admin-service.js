
function tagDuplicate(formName) {
    preloader();
    var val = '';
    if (formName == 'create') {
        val = $('#create-form #UniqueName').val();
    }
    else {
        val = $('#edit-form #UniqueName').val();
    }
    $.post('@Url.Action("TagDuplicate")', { 'value': val }, function (r) {
        UIkit.notification({ message: r.msg, status: r.status, pos: 'bottom-center', timeout: 10000 });
    }, 'json').then(function () {
        setTimeout(function () {
            preloader();
        }, 200);
    });
}
/*function edit(id) {
    preloader();
    $.get('@Url.Action("EditService")', { 'id': id }, function (r) {
        $('#edit-modal').html(r);
    }, 'html').then(function () {
        parseValidator('#edit-modal form');
        setTimeout(function () {
            UIkit.modal('#edit-modal').show();
            preloader();
        }, 200);
    })
}*/
function remove(id) {
    $('#delete-modal input').val(id);
    UIkit.modal('#delete-modal').show();
}
function removeSample(id, sampleType) {
    /*$('#delete-modal input')[0].val(id);
    $('#delete-modal input')[1].val(sampleType);*/
    /*#delete - form*/
    /*$('#delete-modal #sampleType').val(sampleType);*/
    $('#delete-sample-modal #id').val(id);
    UIkit.modal('#delete-modal').show();
}
function opration(id, title) {
    var modal = $('#op-modal');
    modal.find('.uk-modal-title').text(title);
    modal.find('#gallery').attr('href', '/admin/product-gallery/' + id);
    modal.find('#tags').attr('onclick', 'getTags(' + id + ', "' + title + '")');
    modal.find('#attr').attr('href', '/admin/product-attr/' + id);
    modal.find('#variant').attr('href', '/admin/product-variant/' + id);
    UIkit.modal('#op-modal').show();
}
function toggleVariants(t) {
    event.stopPropagation();
    $(t).find('i').toggleClass('fa-plus-circle').toggleClass('fa-minus-circle');
    $(t).parents('tr').next('.detail-row').fadeToggle(400);
}
function getTags(id, title) {
    preloader();
    $('#tag-modal #prdId').val(id);
    $.post('/admin/get-product-tags', { 'id': id }, function (r) {
        var allTags = r.allTags;
        var currentTags = r.currentTags;

        console.log(allTags);
        console.log(currentTags);

        $('#tag-modal h3').text('برچسب های محصول (' + title + ')');
        $('#currentTags ul').html('');
        $.each(currentTags, function (i, v) {
            var li = '<li><span class="far fa-times-circle pointer" data-id="' + v.productTagId + '" onclick="removeTag(this)" ></span><span class="xs uk-margin-small-right">' + v.tagName + '</span></li>';
            $('#currentTags ul').append(li);
        });

        $('#allTags').html('');
        $('#allTags').append('<option value="0">----</option>');
        $.each(allTags, function (i, v) {
            var opt = '<option value="' + v.id + '">' + v.name + '</option>';
            $('#allTags').append(opt);
        });
        UIkit.modal('#tag-modal').show();
    }, 'json').then(function () {
        preloader();
    });
}
function removeTag(t) {
    preloader();
    var tagId = $(t).data('id');
    $.post('/admin/remove-product-tag', { 'id': tagId }, function (r) {
        if (r == true) {
            $(t).parent().fadeOut(500);
        } else {
            alert('خطای سرور در هنگام حذف تگ رخ داده است. لطفا چند دقیقه دیگر امتحان کنید.');
        }
    }, 'json').then(function () {
        preloader();
    });
}
function addTag(t) {
    var val = $(t).val();
    if (val > 0) {
        preloader();
        var productId = $('#prdId').val();
        $.post('/admin/add-product-tag', { 'tagId': val, 'productId': productId }, function (r) {
            if (r > 0) {
                var name = $('option:selected', t).text();
                $('#currentTags ul').append('<li><span class="far fa-times-circle pointer" data-id="' + r + '" onclick="removeTag(this)" ></span><span class="xs uk-margin-small-right">' + name + '</span></li>');
                $('option:selected', t).remove();
            } else if (r == 0) {
                alert('خطای سرور در هنگام افزودن تگ رخ داده است. لطفا چند دقیقه دیگر امتحان کنید.');
            }
        }, 'json').then(function () {
            preloader();
        });
    }
}
function filterSwitch(tag) {
    let value = $(tag).val();
    let html = '';
    switch (value) {
        case 'name':
            html = '<label class="uk-form-label">نام محصول را تایپ کنید</label><input type="text" name="param" class="uk-input uk-border-rounded" />'
            break;
        case 'code':
            html = '<label class="uk-form-label">کد محصول را تایپ کنید</label><input type="number" name="param" class="uk-input uk-border-rounded" />'
            break;
        case 'discountRange':
            html = '<div class="uk-grid-medium uk-child-width-1-1 uk-child-width-1-2@m" uk-grid><div><label>از</label><input class="uk-input uk-border-rounded" name="param1" type="number" min="0" max="100" /></div><div><label>تا</label><input class="uk-input uk-border-rounded" name="param2" type="number" min="0" max="100" /></div></div>'
            break;
        case 'tag':
            preloader();
            html = '<label class="uk-form-label">تگ مورد نظر خود را انتخاب کنید</label><select class="uk-select uk-border-rounded" id="param-select" name="param"></select>';
            $('#append-filter').html(html);
            $.post('@Url.Action("GetAllTags")', {}, function (r) {
                $.each(r, function (i, v) {
                    $('#param-select').append('<option value="' + v.id + '">' + v.name + '</option>');
                });
            }, 'json').then(function () {
                preloader();
            });
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
        $.post('/admin/product', data, function (r) {
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
    $(t).parents('tr').next('.detail-row').fadeToggle(200);
}
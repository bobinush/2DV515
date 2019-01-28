// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
$('.nav-tabs').click(function (e) {
    var target = '';
    if (e.target.hash.includes('pearson')) {
        target = 'pearson';
    }
    if (e.target.hash.includes('euclidean')) {
        target = 'euclidean';
    }
    var id = $('#UserId').val();
    $.get('Calc' + target + '/' + id, {}, function (response) {
        $('#' + target).html(response);
    });
});
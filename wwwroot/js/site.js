// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
$('.nav-tabs').click(function (e) {
    var minRat = $('#min-recommendations').val();

    var target = '';
    if (e.target.hash.includes('pearson')) {
        target = 'pearson';
    }
    if (e.target.hash.includes('euclidean')) {
        target = 'euclidean';
    }
    var id = $('#UserId').val();
    var intervalId = doLoading(target);
    $.get('Calc' + target + '/' + id, { minRatings: minRat }, function (response) {
        clearLoading(intervalId);
        $('#' + target).html(response);
    });
});

function doLoading(target) {
    var originalText = 'Loading';
    var i = 0;
    var id = setInterval(function () {
        $('#' + target).append('.');
        i++;
        if (i == 4) {
            $('#' + target).html(originalText);
            i = 0;
        }
    }, 500);
    $('#' + target).text('Loading');
    return id;
}

function clearLoading(id) {
    clearInterval(id);
}
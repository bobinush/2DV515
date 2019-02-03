$(document).ready(function () {
    $(document).on('click', '.recommendations', function (e) {
        var minRat = $('#min-rec').val();

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
        $('#' + target).text(originalText);
        return id;
    }

    function clearLoading(id) {
        clearInterval(id);
    }
    $("#ratings").on('click', '#prev', function () {
        var _this = $(this);
        var prevPage = _this.data('id') - 1;
        var url = _this.data('url');

        $.get(url, { page: prevPage }, function (response) {
            $('#ratings').html(response);
        });
    });

    $("#ratings").on('click', '#next', function () {
        var _this = $(this);
        var nextPage = _this.data('id') + 1;
        var url = _this.data('url');

        $.get(url, { page: nextPage }, function (response) {
            $('#ratings').html(response);
        });
    });
});
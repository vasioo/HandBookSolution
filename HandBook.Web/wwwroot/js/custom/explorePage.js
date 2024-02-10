$(document).ready(function () {
    $('.explore-link').click(function (e) {
        e.preventDefault();
        var itemId = $(this).data('explore-id');

        $.get('/Home/GetSpecificExplorePageItems', { itemId: itemId }, function (response) {
            var overlay = $('<div class="overlay"></div>');
            var container = $('<div class="container-explorer"></div>').append(response);
            overlay.append(container);
            $('body').append(overlay);
        });

    });
});
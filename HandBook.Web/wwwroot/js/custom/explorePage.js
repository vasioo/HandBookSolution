$(document).ready(function () {

    $('.explore-link').click(function (e) {
        e.preventDefault();
        var itemId = $(this).data('explore-id');

        $.get('/Home/GetSpecificExplorePageItems', { itemId: itemId }, function (response) {
            var overlay = $('<div class="overlay"></div>');
            var container = $('<div class="container-explorer"></div>').append(response);
            overlay.append(container);
            $('body').append(overlay);

            $(document).on('click', function (event) {
                if (!$(event.target).closest('.card').length) {
                    overlay.remove();
                    $(document).off('click');
                }
            });
        });
        return false;
    });

    $(document).ready(function () {
        var searchInput = $('#searchInput');
        var explorePage = $("#explore-page");

        searchInput.on('input', function () {
            var searchString = searchInput.val();
            searchUsers(searchString);
        });

        searchInput.on("input", function () {
            if (searchInput.val().trim() !== "") {
                explorePage.css('display', 'none');
                $('#clearSearch').css('display', 'block');
                $('#hidden-icon-for-canceling').css('display', 'block');
            } else {
                $('#clearSearch').css('display', 'none');
                $('#hidden-icon-for-canceling').css('display', 'none');
                explorePage.css('display', 'block');
            }
        });
    });

    $(window).on('load', function () {
        var searchInput = $('#searchInput');
        var searchStringBeggining = searchInput.val();
        searchUsers(searchStringBeggining);
    });

    function searchUsers(query) {
        var xhr = new XMLHttpRequest();
        xhr.onreadystatechange = function () {
            if (xhr.readyState === XMLHttpRequest.DONE) {
                if (xhr.status === 200) {
                    var users = JSON.parse(xhr.responseText);
                    var resultsContainer = $('#searchResults');
                    resultsContainer.empty();
                    users.forEach(function (user) {
                        var usernameLink = 'https://res.cloudinary.com/dzaicqbce/image/upload/v1695818842/profile-image-for-' + user.userName + '.png';
                        var alternativeLink = '/handbook/images/anonymousUser.png';
                        var imgTag = '<img src="' + usernameLink + '" alt="Image not found" onerror="this.onerror=null;this.src=\'' + alternativeLink + '\';" style="border-radius:30px;height:60px; width:60px; margin-right:10px;" />';

                        var userDiv = $('<div class="container">' +
                            '<div class="row justify-content-center pt-1">' +
                            '   <div class="col-8 border">' +
                            '       <a class="user-link" href="/Home/Account?username=' + user.userName + '">' +
                            '           <div class="row align-items-center user-information">' +
                            '               <div class="col-1 pt-1 pb-1">' +
                            '                   ' + imgTag +
                            '               </div>' +
                            '               <div class="col">' +
                            '                   <div class="row p-0 m-0 h5">' +
                            '                       ' + user.userName +
                            '                   </div>' +
                            '               </div>' +
                            '           </div>' +
                            '       </a>' +
                            '   </div>' +
                            '</div>' +
                            '</div>');

                        resultsContainer.append(userDiv);
                    });
                } else {

                }
            }
        };

        if (query === "") {
            var resultsContainer = $('#searchResults');
            resultsContainer.empty();
            return;
        }
        xhr.open('GET', '/Home/SearchUsersFilter?searchString=' + query, true);
        xhr.send();
    }

    $('#clearSearch').click(function (event) {
        event.preventDefault();
        $('#searchInput').val('');
        $('#searchResults').html('');
        $("#explore-page").css('display', 'block');
        $('#clearSearch').css('display', 'none');
        $('#hidden-icon-for-canceling').css('display', 'none');
    });

    $('#hidden-icon-for-canceling').click(function (event) {
        event.preventDefault();
        $('#searchInput').val('');
        $('#searchResults').html('');
        $("#explore-page").css('display', 'block');
        $('#clearSearch').css('display', 'none');
        $('#hidden-icon-for-canceling').css('display', 'none');
    });
});

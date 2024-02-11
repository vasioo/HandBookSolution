$(document).ready(function () {

    $(document).on('click', '.submit-btn', function () {
        var $commentContainer = $(this).closest('.comment-section');
        var $commentTextarea = $commentContainer.find('.comments-text');

        var commentContent = $commentTextarea.val();
        var postId = $commentContainer.data('post-id');

        var comment = {
            CommentContent: commentContent,
            DateOfCreation: new Date().toString(),
            PostId: postId
        };

        $.ajax({
            url: "/Home/AddOrRemoveAComment",
            method: 'POST',
            data: { commentsDTO: comment },
            dataType: 'json',
            success: function (response) {
                var commentDto = JSON.parse(response);

                var usernameLink = 'https://res.cloudinary.com/dzaicqbce/image/upload/v1695818842/profile-image-for-' + commentDto.UserUsername + '.png';
                var alternativeLink = "/handbook/images/anonymousUser.png";
                var imgTag = '<img src="' + usernameLink + '" alt="Image not found" onerror="this.onerror=null;this.src=\'' + alternativeLink + '\';" style="border-radius:30px; width:60px;height:60px; margin-right:10px;" />';

                var commentHtml = `
            <div class="comment-container pt-2">
                <div class="profile-column">
                   `+ imgTag + `
                </div>
                <div class="comment-column">
                    <div class="comment">
                        <div class="comment-header">
                            <div class="comment-username">${commentDto.UserUsername}</div>
                            <div class="comment-time">Now</div>
                        </div>
                        <div class="comment-content">${commentDto.CommentContent}</div>
                        <div class="comment-actions">
                            <a href="#" class="comment-action">Like</a>
                            <a href="#" class="comment-action">Reply</a>
                        </div>
                    </div>
                    <div class="replies-container">
                        <!-- Add replies here if available -->
                    </div>
                    <div class="view-replies">View Replies</div>
                </div>
            </div>
        `;

                $('.comment-section-regulation-div').prepend(commentHtml);


            },
            error: function (error) {
                console.error('Error submitting comment:', error);
            }
        });
    });

    $(document).on('click', '.submit-reply-btn', function () {
        var $commentContainer = $(this).closest('.input-group');

        // Find the replies-container inside the comment-container
        var $repliesContainer = $commentContainer.closest('.replies-container');

        // Get the ID from the replies-container
        var commentDeriveFromId = $repliesContainer.attr('id');

        // Rest of your existing code
        var $commentTextarea = $commentContainer.find('.replies-text');
        var commentContent = $commentTextarea.val();
        var postId = $commentContainer.data('post-id');

        var comment = {
            CommentContent: commentContent,
            DateOfCreation: new Date().toString(),
            PostId: postId,
            CommentDeriveFromId: commentDeriveFromId
        };


        $.ajax({
            url: "/Home/AddOrRemoveAComment",
            method: 'POST',
            data: { commentsDTO: comment },
            dataType: 'json',
            success: function (response) {
                var commentDto = JSON.parse(response);

                var usernameLink = 'https://res.cloudinary.com/dzaicqbce/image/upload/v1695818842/profile-image-for-' + commentDto.UserUsername + '.png';
                var alternativeLink = "/handbook/images/anonymousUser.png";
                var imgTag = '<img src="' + usernameLink + '" alt="Image not found" onerror="this.onerror=null;this.src=\'' + alternativeLink + '\';" style="border-radius:30px; width:60px;height:60px; margin-right:10px;" />';

                var commentHtml = `
            <div class="comment-container pt-2">
                <div class="profile-column">
                   `+ imgTag + `
                </div>
                <div class="profile-column">
                    <div class="comment">
                        <div class="comment-header">
                            <div class="comment-username">${commentDto.UserUsername}</div>
                            <div class="comment-time">Now</div>
                        </div>
                        <div class="comment-content">${commentDto.CommentContent}</div>
                        <div class="comment-actions">
                            <a href="#" class="comment-action">Like</a>
                            <a href="#" class="comment-action">Reply</a>
                        </div>
                    </div>
                </div>
            </div>
        `;

                var replyContainerId = "container-" + commentDto.CommentDeriveFromId;
                var $repliesContainer = $('#' + replyContainerId);

                $repliesContainer.prepend(commentHtml);
            },
            error: function (error) {
                // Add logging
                console.error('Error submitting comment:', error);
            }
        });
    });

    function likeButtonClick(Id) {
        $.ajax({
            type: "POST",
            url: "/Home/IncrementOrDecrementLikeCount",
            data: { itemId: Id },
            success: function (result) {
                var likeCountElement = $('.likeCount[data-item-id="' + Id + '"]');
                likeCountElement.html(result + ' <i class="fa-solid fa-heart liked fa-sm" style="color: #ff0000;"></i>');
                likeCountElement.toggle(result > 0);
            },
            error: function (result) {
                alert("Error: " + result.statusText);
            }
        });
    }

    function toggleComments(element, itemId) {
            const card = element.closest('.card');
            const cardId = `card-overlay-${itemId}`;

            var postId = $(card).data('post-id');

            loadMoreComments(0, postId);

            const existingOverlay = document.getElementById(cardId);

            if (existingOverlay) {
                existingOverlay.remove();
                const commentSection = card.querySelector('.comment-section');
                commentSection.style.display = 'none';

                card.style.zIndex = '';
                card.style.position = '';

                $('.comment-section-regulation-div').empty();

                document.removeEventListener('click', handleClickOutside);
                return;
            }

            const overlay = document.createElement('div');
            overlay.id = cardId;

            function handleClickOutside(event) {
                if (event.target === overlay) {
                    card.style.zIndex = '';
                    card.style.position = ''; // Reset card position
                    overlay.remove();

                    const commentSection = card.querySelector('.comment-section');
                    commentSection.style.display = 'none';

                    document.removeEventListener('click', handleClickOutside);
                }
            }

            card.style.zIndex = 1000;
            card.style.position = 'relative'; 

            overlay.style.position = 'fixed';
            overlay.style.top = '0';
            overlay.style.left = '0';
            overlay.style.width = '100%';
            overlay.style.height = '100%';
            overlay.style.backgroundColor = 'rgba(0, 0, 0, 0.7)';
            overlay.style.backdropFilter = 'blur(5px)'; 

            const commentSection = card.querySelector('.comment-section');

            if (commentSection.style.display === 'none' || commentSection.style.display === '') {
                commentSection.style.display = 'block';

                card.scrollIntoView({ behavior: 'smooth', block: 'start', inline: 'nearest' });
            } else {
                commentSection.style.display = 'none';
            }

            document.body.appendChild(overlay);

            document.addEventListener('click', handleClickOutside);
        }

    function like(button, Id) {
        var card = button.closest(".card");
        var cardId = card.dataset.id;
        var count = parseInt(button.dataset.count) || 0;
        var likedCards = JSON.parse(sessionStorage.getItem("likedCards")) || {};

        var heartIcon = button.querySelector("i.fa-heart");


        if (heartIcon.classList.contains("liked")) {
            count--;
            button.dataset.count = count;
            button.innerHTML = ` <i class="fa-regular fa-heart fa-xl" style="color: #000;"></i>`;
            likeButtonClick(Id);
            delete likedCards[cardId];
        } else {
            count++;
            button.dataset.count = count;
            button.innerHTML = ` <i class="fa-solid fa-heart fa-xl liked" style="color: #ff0000;"></i>`;
            likeButtonClick(Id);
            likedCards[cardId] = true;
        }

        sessionStorage.setItem("likedCards", JSON.stringify(likedCards));
    }

    function toggleReplies(element) {
        var commentId = element.closest('.comment-container')
        var commentNeededId = $(commentId).data('comment-id');

        const card = element.closest('.card');
        var postId = $(card).data('post-id');

        loadMoreComments(commentNeededId, postId);
        var repliesContainer = element.nextElementSibling;
        if (repliesContainer.style.display === 'none') {
            repliesContainer.style.display = 'block';
        } else {
            var neededID = `container-${commentNeededId}`;

            $(`#${neededID}`).empty();
            repliesContainer.style.display = 'none';

        }
    }

    function likecom(button, Id) {
        var card = button.closest(".comment");
        var cardId = card.dataset.id;
        var count = parseInt(button.dataset.count) || 0;
        var likedComments = JSON.parse(sessionStorage.getItem("likedComments")) || {};

        var heartIcon = button.querySelector(".com-btn");


        if (heartIcon.classList.contains("liked")) {
            count--;
            button.dataset.count = count;
            button.innerHTML = `<div class="com-btn">Like</div>`;
            likeCommentClick(Id);
            delete likedComments[cardId];
        } else {
            count++;
            button.dataset.count = count;
            button.innerHTML = `<div class="liked com-btn">Liked</div>`;
            likeCommentClick(Id);
            likedComments[cardId] = true;
        }

        sessionStorage.setItem("likedComments", JSON.stringify(likedComments));
    }

    function likeCommentClick(Id) {
        $.ajax({
            type: "POST",
            url: "/Home/IncrementOrDecrementCommentLikeCount",
            data: { itemId: Id },
            success: function (result) {
                var likeCountElement = $('.commentlikeCount[data-item-id="' + Id + '"]');
                likeCountElement.html(result + ' <i class="fa-solid fa-heart liked fa-sm" style="color: #ff0000;"></i>');
                likeCountElement.toggle(result > 0);
            },
            error: function (result) {
                alert("Error: " + result.statusText);
            }
        });
    }

    var offsetPost = 0;
    var loadingPost = false;

    $(document).on('click', '.commentButton', function () {
        var itemId = this.id;
        toggleComments(this, itemId);
    });

    $(document).on('click', '.likeButton', function () {
        var itemId = this.id;
        like(this, itemId);
    });


    var offset = 0;
    var loading = false;

    $(window).scroll(function () {
        if ($(window).scrollTop() + $(window).height() >= $(".postContainer").height() - 200 && !loading) {
            loading = true;
            loadMorePosts();
        }
    });

    function loadMorePosts() {
        $.ajax({
            type: "POST",
            url: "/Home/LoadMorePosts",
            data: { offset: offsetPost },
            success: function (data) {
                var posts = JSON.parse(data);

                if (posts.length > 0) {
                    offsetPost += posts.length;

                    posts.forEach(function (post) {
                        var postString = '';
                        offsetPost += posts.length;

                        var link = "https://res.cloudinary.com/dzaicqbce/image/upload/v1695818842/profile-image-for-" + post.CreatorUserName + ".png";

                        var imgTag = '<img src="' + link + '" class="profile-image-class" alt="Image not found" style="border-radius:30px; width:60px;height:60px; margin-right:10px;" />';


                        var postHtml = '<div class="card col-7 mt-3" data-post-id="' + post.Id + '">' +
                            '<div class="card-header border-bottom mb-2">' +
                            '<div class="row justify-content-between w-100">' +
                            '<div class="profile-column-post d-flex align-items-center ">' +
                            '<a class="" asp-controller="Home" asp-action="Account" asp-route-username="' + post.CreatorUserName + '">' +
                            imgTag +
                            '</a>' +
                            '<div>' +
                            '<a class="card-title h5 cr-us-name" asp-controller="Home" asp-action="Account" asp-route-username="' + post.CreatorUserName + '">' +
                            '&#64;' + post.CreatorUserName +
                            '</a>' +
                            '<div class="custom-date">' + getTimeDisplay(post.Time) + '</div>' +
                            '</div>' +
                            '</div>' +
                            '</div>' +
                            '</div>' +

                            '<img src="data:image;base64,' + base64Image + '" style="border-radius:6px; height:500px" alt="Image" class="fit-image">' +
                            '<div class="card-body p-0 m-0">' +
                            '<div class="row justify-content-center p-0 m-0">' +
                            '<div class="col-10 pt-3">' +
                            '<div class="d-flex justify-content-between">' +
                            '<div>' +
                            '<span class="likeCount" data-item-id="' + post.Id + '" style="display: ' + (post.AmountOfLikes > 0 ? 'block' : 'none') + '">' +
                            post.AmountOfLikes + ' <i class="fa-solid ' + likeButtonIcon + '" style="color: #ff0000;"></i>' +
                            '</span>' +
                            '</div>' +
                            '<div>' +
                            '<span class="commentsCount" data-item-id="' + post.Id + '">' + commentsCount + ' ' + commentsText + '</span>' +
                            '</div>' +
                            '</div>' +
                            '</div>' +
                            '</div>' +

                            '<div class="d-flex justify-content-center">' +
                            '<hr class="col-11">' +
                            '</div>' +

                            '<div class="row">' +
                            '<div class="col-12 d-flex justify-content-around">' +
                            '<div class="col-1 pt-2 text-center pb-3">' +
                            '<button class="likeButton" onclick="like(this,' + post.Id + ')" data-count="' + post.AmountOfLikes + '">' +
                            '<i class="fa-regular ' + likeButtonIcon + '" style="color: #000;"></i>' +
                            '</button>' +
                            '<br>' +
                            '</div>' +
                            '<div class="col-1 pt-2 text-center pb-3">' +
                            '<button class="commentButton" onclick="toggleComments(' + post.Id + ', this)">' +
                            '<i class="fa-regular fa-comment fa-xl"></i>' +
                            '</button>' +
                            '<br>' +
                            '</div>' +
                            '<div class="col-1 pt-2 text-center pb-3">' +
                            '<button class="shareButton" onclick="share(this,' + post.Id + ')">' +
                            '<i class="fa-solid fa-share-from-square fa-xl"></i>' +
                            '</button>' +
                            '<br>' +
                            '</div>' +
                            '</div>' +
                            '</div>' +
                            '<div class="row comment-section" style="display:none" data-post-id="' + post.Id + '">' +
                            '<div class="col">' +
                            '<div class="col">' +
                            '<div class="input-group">' +
                            '<div class="border col-10 pl-1 pr-1 m-0">' +
                            '<textarea class="comments-text w-100" style="max-height:400px" placeholder="Write a comment..."></textarea>' +
                            '</div>' +
                            '<div class="input-group-append align-self-end col-2">' +
                            '<button class="btn btn-primary submit-btn" type="button"> <i class="fas fa-right-long"></i></button>' +
                            '</div>' +
                            '</div>' +
                            '</div>' +
                            '</div>' +
                            '<div class="comment-section-regulation-div">' +
                            '</div>' +
                            '<div class="">' +
                            '<a class="load-more-comments">Load More Comments</a>' +
                            '</div>' +
                            '</div>' +
                            '</div>' +
                            '</div>';


                        $(".post-container").append(postString);
                    });
                }

                loadingPost = false;
            },
            error: function (error) {
                console.log("Error loading more posts: " + error.responseText);
                loadingPost = false;
            }
        });
    }

    function getTimeDisplay(postDate) {
        var timeDifference = new Date() - new Date(postDate);

        var seconds = Math.floor(timeDifference / 1000);
        var minutes = Math.floor(timeDifference / (1000 * 60));
        var hours = Math.floor(timeDifference / (1000 * 60 * 60));
        var days = Math.floor(timeDifference / (1000 * 60 * 60 * 24));

        var displayString = "";

        if (days > 365 || (days > 7 && new Date(postDate).getFullYear() !== new Date().getFullYear())) {
            var options = { year: 'numeric', month: 'numeric', day: 'numeric', hour: 'numeric', minute: 'numeric', second: 'numeric' };
            displayString = new Date(postDate).toLocaleDateString('en-US', options);
        } else if (days > 7) {
            displayString = new Date(postDate).toLocaleDateString('en-US', { month: 'long', day: 'numeric' });
        } else if (days >= 1) {
            displayString = days === 1 ? "1 day ago" : `${Math.floor(days)} days ago`;
        } else if (hours >= 1) {
            displayString = hours === 1 ? "1 hour ago" : `${Math.floor(hours)} hours ago`;
        } else if (minutes >= 1) {
            displayString = minutes === 1 ? "1 minute ago" : `${Math.floor(minutes)} minutes ago`;
        } else {
            displayString = seconds === 1 ? "1 second ago" : `${Math.floor(seconds)} seconds ago`;
        }

        return displayString;
    }

    var loadingComments = false;

    $(document).on('click', '.load-more-comments', function () {

        const card = $(this).closest('.card');
        var postId = $(card).data('post-id');

        loadMoreComments(0, postId);
    });

    $(document).on('click', '.load-more-replies', function () {
        var commentId = $(this).closest('.comment-container').data('comment-id');

        const card = $(this).closest('.card');
        var postId = $(card).data('post-id');

        loadMoreComments(commentId, postId);
    });

    function loadMoreComments(derivingFromId, postAttrId) {

        var concOffset = offset;

        if (derivingFromId > 0) {
            var concOffset = $('.profile-reply-image-class').length;
        }


        $.ajax({
            type: "POST",
            url: "/Home/LoadMoreComments",
            data: { offset: concOffset, derivingFrom: derivingFromId, postId: postAttrId },
            success: function (data) {

                var posts = JSON.parse(data);

                var commentsList = $(".liked-comments-from-temp").data('comments-list');

                if (posts.length > 0) {
                    var alternativeLink = "/handbook/images/anonymousUser.png";
                    if (derivingFromId > 0) {
                        posts.forEach(function (post) {

                            var replyLink = "https://res.cloudinary.com/dzaicqbce/image/upload/v1695818842/profile-image-for-" + post.AppUser.UserName + ".png";

                            var imgTag = '<img src="' + replyLink + '" class="profile-image-class" alt="Image not found" style="border-radius:30px; width:60px;height:60px; margin-right:10px;" />';

                            var isliked = '<div class="com-btn">Like</div>';
                            if (commentsList && commentsList.includes(post.Id)) {
                                isliked = '<div class="liked com-btn">Liked</div>';
                            }

                            var repliesHtml = '<div class="comment-container pt-2">' +
                                '<div class="profile-column">' +
                                imgTag +
                                '</div>' +
                                '<div class="comment-column">' +
                                '<div class="comment">' +
                                '<div class="comment-header">' +
                                '<div class="comment-username font-weight-bold">' + post.AppUser.UserName + '</div>' +
                                '<div class="comment-time text-muted">' +
                                getTimeDisplay(post.DateOfCreation) +
                                '</div>' +
                                '</div>' +
                                '<div class="comment-content mt-2">' +
                                '<div class="row">' +
                                post.CommentContent +
                                '</div>' +
                                '<div class="row">' +
                                '<span class="commentlikeCount" data-item-id="' + post.Id + '" style="display: ' + (post.AmountOfLikes > 0 ? 'block' : 'none') + ';">' +
                                post.AmountOfLikes + ' <i class="fa-solid fa-heart liked fa-sm" style="color: #ff0000;"></i>' +
                                '</span>' +
                                '</div>' +
                                '</div>' +
                                '<div class="comment-actions mt-2">' +
                                '<button class="comment-like-button text-decoration-none text-primary" onclick="likecom(this,' + post.Id + ')" data-count="' + post.Post.AmountOfLikes + '">' +
                                isliked +
                                '</button>' +
                                '</div>' +
                                '</div>' +
                                '</div>' +
                                '</div>' +
                                '</div>';
                            var containerId = 'container-' + post.CommentDeriveFromId;

                            $('.' + containerId).append(repliesHtml);


                            $('.profile-image-class').on('error', function (event) {
                                $(this).attr('src', 'handbook/images/anonymousUser.png');
                                $(this).off('error');
                            });
                        });
                    }
                    else {
                        offset += posts.length;
                        posts.forEach(function (post) {
                            var link = 'https://res.cloudinary.com/dzaicqbce/image/upload/v1695818842/profile-image-for-' + post.AppUser.UserName + '.png';
                            var imgTag = '<img src="' + link + '" class="profile-image-class" alt="Image not found" style="border-radius:30px; width:60px;height:60px; margin-right:10px;" />';


                            var isliked = '<div class="com-btn">Like</div>';
                            if (commentsList && commentsList.includes(post.Id)) {
                                isliked = '<div class="liked com-btn">Liked</div>';
                            }

                            var htmlString = '<div class="comment-container pt-2" data-comment-id="' + post.Id + '">' +
                                '<div class="profile-column">' +
                                imgTag +
                                '</div>' +
                                '<div class="comment-column">' +
                                '<div class="comment">' +
                                '<div class="comment-header">' +
                                '<div class="comment-username font-weight-bold">' + post.AppUser.UserName + '</div>' +
                                '<div class="comment-time text-muted">' +
                                getTimeDisplay(post.DateOfCreation) +
                                '</div>' +
                                '</div>' +
                                '<div class="comment-content mt-2">' +
                                '<div class="">' +
                                post.CommentContent +
                                '</div>' +
                                '<div class="row">' +
                                '<span class="commentlikeCount" data-item-id="' + post.Id + '" style="display: ' + (post.AmountOfLikes > 0 ? 'block' : 'none') + ';">' +
                                post.AmountOfLikes +
                                ' <i class="fa-solid fa-heart liked fa-sm" style="color: #ff0000;"></i>' +
                                '</span>' +
                                '</div>' +
                                '</div>' +
                                '<div class="comment-actions mt-2">' +
                                '<button class="comment-like-button text-decoration-none text-primary" onclick="likecom(this,' + post.Id + ')" data-count="' + post.Post.AmountOfLikes + '">' +
                                isliked +
                                '</button>' +
                                '</div>' +
                                '<a class="view-replies mt-2 text-primary" onclick="toggleReplies(this)">View Replies </a>' +
                                '<div class="p-3 m-1 replies-container" id="' + post.Id + '" style="display:none">' +
                                '<div class="input-group mt-3" data-post-id="' + post.Post.Id + '">' +
                                '<div class="border col-10 p-0">' +
                                '<textarea class="replies-text form-control" style="max-height:400px" placeholder="Reply to ' + post.AppUser.UserName + '...."></textarea>' +
                                '</div>' +
                                '<div class="input-group-append col-2">' +
                                '<button class="btn btn-primary submit-reply-btn" type="button"><i class="fas fa-right-long"></i></button>' +
                                '</div>' +
                                '</div>' +
                                '<div class="container-' + post.Id + '">' +
                                '</div>' +
                                '<div class="">' +
                                '<a class="load-more-replies">Load More Replies</a>' +
                                '</div>' +
                                '</div>' +
                                '</div>' +
                                '</div>';
                            $(".comment-section-regulation-div").append(htmlString);

                            $('.profile-image-class').on('error', function (event) {
                                $(this).attr('src', 'handbook/images/anonymousUser.png');
                                $(this).off('error');
                            });
                        });
                    }
                }
                if (posts.length < 20) {
                    if (derivingFromId > 0) {
                        $('.load-more-replies').hide();
                    }
                    else {
                        $('.load-more-comments').hide();
                    }
                }
                loading = false;
            },
            error: function (error) {
                console.log("Error loading more comments: " + error.responseText);
                loading = false;
            }
        });
    }

    function checkImage(url) {
        return fetch(url, { method: 'HEAD' })
            .then(res => res.ok)
            .catch(error => {
                console.error('Error checking image:', error);
                return false;
            });
    }

    $('.profile-image-class').on('error', function () {
        $(this).off('error').attr('src', '/handbook/images/anonymousUser.png');
    }).on('load', function () {
    });


    $('.profile-image-class').each(function () {
        if (!this.complete || (typeof this.naturalWidth !== 'undefined' && this.naturalWidth === 0)) {
            $(this).trigger('error');
        }
    });
});

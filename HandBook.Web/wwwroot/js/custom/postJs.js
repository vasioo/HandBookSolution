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
            <div class="comment-container row pt-2">
                <div class="profile-column">
                   `+ imgTag + `
                </div>
                <div class="comment-column">
                    <div class="comment">
                        <div class="comment-header">
                            <div class="comment-username">${commentDto.userUsername}</div>
                            <div class="comment-time">Now</div>
                        </div>
                        <div class="comment-content">${commentDto.commentContent}</div>
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

                var neededId = `#comment-container-${commentDto.postId}`;
                $('neededId').prepend(commentHtml);


            },
            error: function (error) {
                console.error('Error submitting comment:', error);
            }
        });
    });

    $(document).on('click', '.submit-reply-btn', function () {
        var $commentContainer = $(this).closest('.replies-group');

        var $repliesContainer = $(this).closest('.comment-container').find('.commentlikeCount');

        var commentDeriveFromId = $repliesContainer.data('item-id');

        var $commentTextarea = $commentContainer.find('.reply-text');
        var commentContent = $commentTextarea.val();
        var postId = $(this).closest('.comment-section').data('post-id');

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
                <div class="comment-container row pt-2">
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
            card.style.overflow = '';
            card.style.scroll
            document.body.style.overflow = 'auto';
            $('.comment-section-regulation-div').empty();

            document.removeEventListener('click', handleClickOutside);

            return;
        }

        const overlay = document.createElement('div');
        overlay.id = cardId;

        function handleClickOutside(event) {
            if (event.target === overlay) {
                card.style.zIndex = '';
                card.style.position = '';
                card.style.overflow = '';
                document.body.style.overflow = 'auto';
                overlay.remove();

                const commentSection = card.querySelector('.comment-section');
                commentSection.style.display = 'none';

                document.removeEventListener('click', handleClickOutside);

            }
        }

        card.style.zIndex = 1000;
        card.style.overflowY = 'scroll';
        card.style.height = '100vh';
        document.body.style.overflow = 'hidden';
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

            const cardContainer = card.parentElement;
            cardContainer.style.overflow = 'hidden';
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

    $(document).on('click', '.toggle-replies-btn', function () {
        var commentContainer = $(this).closest('.comment-container');
        var commentId = commentContainer.data('comment-id');

        const card = this.closest('.card');
        var postId = $(card).data('post-id');

        var repliesContainer = this.nextElementSibling;

        if (!repliesContainer.classList.contains('comments-loaded')) {
            loadMoreComments(commentId, postId);
            repliesContainer.classList.add('comments-loaded');
        }

        $(repliesContainer).toggle();
    });

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

                        var link = "https://res.cloudinary.com/dzaicqbce/image/upload/v1695818842/profile-image-for-" + post.creatorUserName + ".png";

                        var imgTag = '<img src="' + link + '" class="profile-image-class" alt="Image not found" style="border-radius:30px; width:60px;height:60px; margin-right:10px;" />';


                        var postHtml = '<div class="card col-7 mt-3" data-post-id="' + post.id + '">' +
                            '<div class="card-header border-bottom mb-2">' +
                            '<div class="row justify-content-between w-100">' +
                            '<div class="profile-column-post d-flex align-items-center ">' +
                            '<a class="" asp-controller="Home" asp-action="Account" asp-route-username="' + post.creatorUserName + '">' +
                            imgTag +
                            '</a>' +
                            '<div>' +
                            '<a class="card-title h5 cr-us-name" asp-controller="Home" asp-action="Account" asp-route-username="' + post.creatorUserName + '">' +
                            '&#64;' + post.creatorUserName +
                            '</a>' +
                            '<div class="custom-date">' + getTimeDisplay(post.time) + '</div>' +
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
                            '<span class="likeCount" data-item-id="' + post.id + '" style="display: ' + (post.amountOfLikes > 0 ? 'block' : 'none') + '">' +
                            post.amountOfLikes + ' <i class="fa-solid ' + likeButtonIcon + '" style="color: #ff0000;"></i>' +
                            '</span>' +
                            '</div>' +
                            '<div>' +
                            '<span class="commentsCount" data-item-id="' + post.id + '">' + commentsCount + ' ' + commentsText + '</span>' +
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
                            '<button class="likeButton" onclick="like(this,' + post.id + ')" data-count="' + post.amountOfLikes + '">' +
                            '<i class="fa-regular ' + likeButtonIcon + '" style="color: #000;"></i>' +
                            '</button>' +
                            '<br>' +
                            '</div>' +
                            '<div class="col-1 pt-2 text-center pb-3">' +
                            '<button class="commentButton" onclick="toggleComments(' + post.id + ', this)">' +
                            '<i class="fa-regular fa-comment fa-xl"></i>' +
                            '</button>' +
                            '<br>' +
                            '</div>' +
                            '<div class="col-1 pt-2 text-center pb-3">' +
                            '<button class="shareButton" onclick="share(this,' + post.id + ')">' +
                            '<i class="fa-solid fa-share-from-square fa-xl"></i>' +
                            '</button>' +
                            '<br>' +
                            '</div>' +
                            '</div>' +
                            '</div>' +
                            '<div class="row comment-section" style="display:none" data-post-id="' + post.id + '">' +
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
                            '<div class="comment-section-regulation-div" id="comment-container-"'+post.id+'">' +
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


    $(document).on('click', '.comment-like-button', function () {
        const comment = $(this).closest('.comment-container').find('.commentlikeCount');
        var postId = comment.data('item-id');


        likecom(this, postId);
    });

    function loadMoreComments(derivingFromId, postAttrId) {
        var concOffset = offset;

        if (derivingFromId > 0) {
            concOffset = $('.profile-reply-image-class').length;
        }

        $.ajax({
            type: "POST",
            url: "/Home/LoadMoreComments",
            data: { offset: concOffset, derivingFrom: derivingFromId, postId: postAttrId },
            success: function (data) {
                var posts = data.message;
                var commentsList = $(".liked-comments-from-temp").data('comments-list');
                var fragment = document.createDocumentFragment();

                if (posts.length > 0) {
                    if (derivingFromId != 0) {
                        posts.forEach(function (comment) {
                            var link = `https://res.cloudinary.com/dzaicqbce/image/upload/v1695818842/profile-image-for-${comment.appUser.userName}.png`;
                            var imgTag = `<img src="${link}" class="profile-image-class" alt="Image not found" style="border-radius:30px; width:60px;height:60px; margin-right:10px;" />`;
                            var isLiked = commentsList && commentsList.includes(comment.id) ? '<div class="liked com-btn">Liked</div>' : '<div class="com-btn">Like</div>';
                            var commentContent = `<div class="d-flex pb-3 pt-2">${comment.commentContent}</div>`;
                            var likeCount = `<span class="commentlikeCount" data-item-id="${comment.id}" style="display: ${comment.amountOfLikes > 0 ? 'block' : 'none'};">${comment.amountOfLikes} <i class="fa-solid fa-heart liked fa-sm" style="color: #ff0000;"></i></span>`;
                            var commentActions = `
                                <div class="comment-actions mt-2">
                                    <button class="comment-like-button text-decoration-none text-primary" data-count="${comment.post.amountOfLikes}">
                                        ${isLiked}
                                    </button>
                                    <button class="append-reply-textbox">
                                        Reply
                                    </button>
                                </div>
                               ${comment.amountOfReplies > 0 ? '<a class="toggle-replies-btn btn row">View Replies</a>' : ''}
                                <div class="sub-replies-container" id="${comment.id}"></div>
                                `;

                            document.addEventListener('click', function (event) {
                                if (event.target.classList.contains('append-reply-textbox')) {
                                    var repliesContainer = event.target.parentElement.nextElementSibling.nextElementSibling;
                                    var inputBoxHtml = `
                                    <div class="col">
                                        <div class="col">
                                            <div class="replies-group">
                                                <div class="border col-10 pl-1 pr-1 m-0">
                                                    <textarea class="reply-text w-100" style="max-height:400px" placeholder="Write a comment..."></textarea>
                                                </div>
                                                <div class="input-group-append align-self-end col-2">
                                                    <button class="btn btn-primary submit-reply-btn" type="button"> <i class="fas fa-right-long"></i></button>
                                                </div>
                                            </div>
                                        </div>
                                    </div>`;
                                    repliesContainer.innerHTML = inputBoxHtml;
                                }
                            });


                            var repliesHtml = `<div class="comment-container row pt-2" data-comment-id="${comment.id}"><div class="profile-column">${imgTag}</div><div class="comment-column"><div class="comment"><div class="comment-header"><div class="comment-username font-weight-bold">${comment.appUser.userName}</div><div class="comment-time text-muted">${getTimeDisplay(comment.dateOfCreation)}</div></div><div class="comment-content mt-2">${commentContent}<div class="row">${likeCount}</div></div>${commentActions}</div></div></div>`;
                            var containerId = `container-${comment.commentDeriveFromId}`;

                            // Append to fragment
                            var tempDiv = document.createElement('div');
                            tempDiv.innerHTML = repliesHtml;
                            fragment.appendChild(tempDiv.firstChild);
                        });

                        var containerId = `#replies-container-${derivingFromId}`;
                        var replContainer = $(containerId);
                        replContainer.css({
                            'overflow-y': 'auto',
                            'overflow-x': 'hidden',
                            'padding': '2.2em',
                            'display':'block'
                        });

                        replContainer.append(fragment);

                    }
                    else {
                        posts.forEach(function (comment) {
                            var link = `https://res.cloudinary.com/dzaicqbce/image/upload/v1695818842/profile-image-for-${comment.appUser.userName}.png`;
                            var imgTag = `<img src="${link}" class="profile-image-class" alt="Image not found" style="border-radius:30px; width:50px;height:50px; margin-right:10px;" />`;
                            var isLiked = commentsList && commentsList.includes(comment.id) ? '<div class="liked com-btn">Liked</div>' : '<div class="com-btn">Like</div>';
                            var commentContent = `<div class="d-flex py-3">${comment.commentContent}</div>`;
                            var likeCount = `<span class="commentlikeCount" data-item-id="${comment.id}" style="display: ${comment.amountOfLikes > 0 ? 'block' : 'none'};">${comment.amountOfLikes} <i class="fa-solid fa-heart liked fa-sm" style="color: #ff0000;"></i></span>`;
                            var commentActions = `
                                <div class="comment-actions mt-2">
                                    <button class="comment-like-button text-decoration-none text-primary" data-count="${comment.post.amountOfLikes}">
                                        ${isLiked}
                                    </button>
                                    <button class="append-reply-textbox">
                                        Reply
                                    </button>
                                </div>
                                ${comment.amountOfReplies > 0 ? '<a class="toggle-replies-btn btn row">View Replies</a>' : ''}
                                <div class="replies-container" style="display:none;" id="replies-container-${comment.id}"></div>
                                `;

                            document.addEventListener('click', function (event) {
                                if (event.target.classList.contains('append-reply-textbox')) {
                                    var repliesContainer = event.target.parentElement.nextElementSibling.nextElementSibling;
                                    var inputBoxHtml = `
                                    <div class="col">
                                        <div class="col">
                                            <div class="replies-group">
                                                <div class="border col-10 pl-1 pr-1 m-0">
                                                    <textarea class="reply-text w-100" style="max-height:400px" placeholder="Write a comment..."></textarea>
                                                </div>
                                                <div class="input-group-append align-self-end col-2">
                                                    <button class="btn btn-primary submit-reply-btn" type="button"> <i class="fas fa-right-long"></i></button>
                                                </div>
                                            </div>
                                        </div>
                                    </div>`;
                                    repliesContainer.innerHTML = inputBoxHtml;
                                }
                            });


                            var repliesHtml = `
                            <div class="comment-container row pt-2" data-comment-id="${comment.id}">
                                <div class="profile-column col-1 mx-2 p-0">
                                    ${imgTag}
                                </div>
                                <div class="comment-column col mx-2 p-0">
                                    <div class="comment">
                                        <div class="comment-header rounded-top" style="background:#B0B3B8; padding-top:0.6em; padding-left:1.2em;">
                                            <div class="comment-username font-weight-bold">
                                                ${comment.appUser.userName}
                                            </div>
                                            <div class="comment-time text-muted">
                                                ${getTimeDisplay(comment.dateOfCreation)}
                                            </div>
                                        </div>
                                        <div class="comment-content rounded-bottom" style="background:#B0B3B8; padding-bottom:0.6em;padding-left:1.2em;">
                                            ${commentContent}
                                            <div class="row">
                                                ${likeCount}
                                            </div>
                                        </div>
                                        ${commentActions}
                                    </div>
                                </div>
                            </div>`;
                            var containerId = `container-${comment.commentDeriveFromId}`;

                            // Append to fragment
                            var tempDiv = document.createElement('div');
                            tempDiv.innerHTML = repliesHtml;
                            fragment.appendChild(tempDiv);
                        });

                        var containerId = `#comment-container-${postAttrId}`
                        var comContainerId = $(containerId);
                        comContainerId.append(fragment);
                    }
                }

                if (posts.length < 20) {
                    if (derivingFromId > 0) {
                        $('.load-more-replies').hide();
                    } else {
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

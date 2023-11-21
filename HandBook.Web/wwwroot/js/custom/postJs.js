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
            var alternativeLink = 'https://res.cloudinary.com/dzaicqbce/image/upload/v1700160046/azgysbpf8xpcxpfclb9i.jpg';
            var imgTag = '<img src="' + usernameLink + '" alt="Image not found" onerror="this.onerror=null;this.src=\'' + alternativeLink + '\';" style="border-radius:30px; width:50px; margin-right:10px;" />';

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
            // Add logging
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
            var alternativeLink = 'https://res.cloudinary.com/dzaicqbce/image/upload/v1700160046/azgysbpf8xpcxpfclb9i.jpg';
            var imgTag = '<img src="' + usernameLink + '" alt="Image not found" onerror="this.onerror=null;this.src=\'' + alternativeLink + '\';" style="border-radius:30px; width:50px; margin-right:10px;" />';

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

function toggleComments(itemId, element) {
    const card = element.closest('.card');
    const cardId = `card-overlay-${itemId}`;

    const existingOverlay = document.getElementById(cardId);

    if (existingOverlay) {
        existingOverlay.remove();
        const commentSection = card.querySelector('.comment-section');
        commentSection.style.display = 'none';

        card.style.zIndex = '';
        card.style.position = ''; // Reset card position

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
    card.style.position = 'relative'; // Adjust card position to maintain its layout in the document flow

    overlay.style.position = 'fixed';
    overlay.style.top = '0';
    overlay.style.left = '0';
    overlay.style.width = '100%';
    overlay.style.height = '100%';
    overlay.style.backgroundColor = 'rgba(0, 0, 0, 0.7)';
    overlay.style.backdropFilter = 'blur(5px)'; // Apply a blur effect to the backdrop

    const commentSection = card.querySelector('.comment-section');

    if (commentSection.style.display === 'none' || commentSection.style.display === '') {
        commentSection.style.display = 'block';

        // Scroll to the top of the card
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

function toggleReplies(link) {
    var repliesContainer = link.nextElementSibling; // Assumes the replies-container is the next sibling
    if (repliesContainer.style.display === 'none') {
        repliesContainer.style.display = 'block';
    } else {
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

var offset = 20;
var loading = false;

var offset = 20; // Initial offset
var loading = false; // Flag to prevent multiple simultaneous requests

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
        data: { offset: offset },
        success: function (data) {
            var posts = JSON.parse(data);

            if (posts.length > 0) {
                offset += posts.length;

                posts.forEach(function (post) {
                    $(".postContainer").append('@await Html.PartialAsync("_PostsPartial", '+post+');');
                });
            }

            loading = false;
        },
        error: function (error) {
            console.log("Error loading more posts: " + error.responseText);
            loading = false;
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

$(window).scroll(function () {
    if ($(window).scrollTop() + $(window).height() >= $(".comment-section").height() - 200 && !loadingComments) {
        loadingComments = true;
        loadMoreComments();
    }
});

function loadMoreComments() {
    $.ajax({
        type: "POST",
        url: "/Home/LoadMoreComments",
        data: { offset: offset, derivingFrom: derivingFrom() },
        success: function (data) {
            var posts = JSON.parse(data);

            if (posts.length > 0) {
                offset += posts.length;

                posts.forEach(function (post) {
                    $(".comment-section-regulation-div").append('@await Html.PartialAsync("_CommentPartial", ' + post + ');');
                });
            }

            loading = false;
        },
        error: function (error) {
            console.log("Error loading more posts: " + error.responseText);
            loading = false;
        }
    });
}

function derivingFrom() {
    var lastItem = $(".comment-section-regulation-div").children().last();
    return lastItem.data("comment-id");
}
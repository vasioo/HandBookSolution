$(document).on('click', '.load-more-comments', function (event) {

    const card = $(this).closest('.card');
    var postId = $(card).data('post-id');

    loadMoreComments(0, postId, event);
});

$(document).on('click', '.load-more-replies', function (event) {
    var commentId = $(this).closest('.comment-container').data('comment-id');

    const card = $(this).closest('.card');
    var postId = $(card).data('post-id');

    loadMoreComments(commentId, postId, event);
});

$(document).on('click', '.comment-like-button', function () {
    const comment = $(this).closest('.comment-container').find('.commentlikeCount');
    var postId = comment.data('item-id');

    likecom(this, postId);
});

$(document).on('click', '.append-reply-textbox', function (event) {
    var $nextSibling = $(this).closest('.comment-actions').next();

    var $replyButton = $(this);
    var $inputBoxContainer = $replyButton.closest('.comment-actions').next('.reply-input-box');

    if (!$nextSibling.hasClass('row')) {
        var inputBoxHtml = `
            <div class="row py-3" style="padding-left: 1.2em;">
                <div class="col" style="background:lightgray;border-radius: 15px;">
                    <div class="col reply-input-box">
                        <div class="replies-group d-flex justify-content-between">
                            <div class="col-11 pl-1 pr-1 m-0">
                                <textarea class="reply-text w-100" placeholder="Reply to..."></textarea>
                            </div>
                            <div class="input-group-append align-self-end">
                                <button class="btn submit-reply-btn" type="button" disabled>
                                    <i class="fa-solid fa-paper-plane" style="rotate:30deg"></i>
                                </button>
                            </div>
                        </div>
                    </div>
                </div>
           </div>`;
        $replyButton.closest('.comment-actions').after(inputBoxHtml);
    } else {
        $nextSibling.remove();
    }
    $inputBoxContainer.toggle();

});


$(document).on('click', '.commentButton', function (event) {
    var itemId = this.id;
    toggleComments(this, itemId, event);
});

$(document).on('click', '.likeButton', function () {
    var itemId = this.id;
    like(this, itemId);
});

$(document).on('click', '.toggle-replies-btn', function () {

    var highestElement = $(this).parents('.comment-container').last();
    var commentId = highestElement.data('comment-id');

    var closestElement = $(this).closest('.comment-container');
    var commentReplyId = closestElement.data('comment-id');

    const card = this.closest('.card');
    var postId = $(card).data('post-id');

    var repliesContainer = this.nextElementSibling;

    if (repliesContainer != null && repliesContainer != undefined) {
        $(repliesContainer).toggle();
        if ($(repliesContainer).is(':visible')) {
            $(this).text('Hide Replies');
        } else {
            $(this).text('View Replies');
        }
    }

    if ($(this).hasClass('no-src')) {
        $('.comment-container[data-deriving-from="' + commentReplyId + '"]').toggle();

        if ($('.comment-container[data-deriving-from="' + commentReplyId + '"]').is(':visible')) {
            $(this).text('Hide Replies');
        } else {
            $(this).text('View Replies');
        }
    }

    var subContainerId = '.commentlikeCount[data-item-id="' + commentReplyId + '"]';
    var subRepliesContainer = $(`${subContainerId}`);

    if (subRepliesContainer.length > 0 && !subRepliesContainer.hasClass('comments-loaded')) {
        loadMoreComments(commentReplyId, postId, $(this));
        subRepliesContainer.addClass('comments-loaded');

    }
});

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

$(document).on('input', '.comment-text', function () {
    var $submitButton = $(this).parent().next().find('.submit-btn');
    if ($(this).val().trim() === '') {
        $submitButton.prop('disabled', true);
    } else {
        $submitButton.prop('disabled', false);
    }
});

$(document).on('input', '.reply-text', function () {
    var $submitButton = $(this).parent().next().find('.submit-reply-btn');

    if ($(this).val().trim() === '') {
        $submitButton.prop('disabled', true);
    } else {
        $submitButton.prop('disabled', false);
    }
});

$('.profile-image-class').on('error', function () {
    $(this).off('error').attr('src', '/handbook/images/anonymousUser.png');
}).on('load', function () { });

$('.profile-image-class').each(function () {
    if (!this.complete || (typeof this.naturalWidth !== 'undefined' && this.naturalWidth === 0)) {
        $(this).trigger('error');
    }
});

$('textarea').on('input', function () {
    this.style.height = 'auto';

    this.style.height =
        (this.scrollHeight) + 'px';
});

$(window).scroll(function () {
    if ($(window).scrollTop() + $(window).height() >= $(".postContainer").height() - 200 && !loading) {
        loading = true;
        loadMorePosts();
    }
});
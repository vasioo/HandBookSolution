function loadMoreComments(derivingFromId, postAttrId, buttonEvent) {
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
                posts.forEach(function (comment, index) {
                    var link = `https://res.cloudinary.com/dzaicqbce/image/upload/v1695818842/profile-image-for-${comment.appUser.userName}.png`;
                    var imgTag = `<img src="${link}" class="profile-image-class" alt="Image not found" style="border-radius:30px; width:50px;height:50px; margin-right:10px;" />`;
                    var isLiked = commentsList && commentsList.includes(comment.id.toLowerCase()) ? '<i class="fa-solid fa-heart liked fa-xl com-btn" style="color: #ff0000;"></i>' : '<i class="fa-regular fa-heart fa-xl com-btn" style="color: #000;"></i>';

                    var commentContent = ``;
                    if (derivingFromId != 0) {
                        var currReplyUsername = $('.comment[data-comment-id="' + derivingFromId + '"]').data('comment-username');
                        commentContent = `
                        <div class="d-flex pb-3 pt-2">
                            <a class="custom-username-link" href="Home/Account?username=${currReplyUsername}">
                                @${currReplyUsername} &#160
                            </a>
                            ${comment.commentContent}
                        </div>`;
                    }
                    else {
                         commentContent = `<div class="d-flex pb-3 pt-2">${comment.commentContent}</div>`;
                    }
                    var likeCount = `<span class="commentlikeCount" data-item-id="${comment.id}" style="display: ${comment.amountOfLikes > 0 ? 'block' : 'none'};">${comment.amountOfLikes} <i class="fa-solid fa-heart liked fa-sm" style="color: #ff0000;"></i></span>`;
                    var commentActions = `
                    <div class="comment-actions mt-2">
                        <a class="comment-like-button btn text-decoration-none text-primary"  data-count="${comment.post.amountOfLikes}">
                            ${isLiked}
                        </a>
                        <a class="append-reply-textbox btn">
                            <i class="fa-solid fa-reply fa-xl"></i>
                        </a>
                    </div>
                    `;
                    if (comment.amountOfReplies > 0) {
                        if (derivingFromId == 0) {
                            commentActions += ` <a class="toggle-replies-btn btn row" data-comment-id="${comment.id}" data-amount-of-replies="${comment.amountOfReplies}">View Replies</a>`;
                        }
                        else {
                            commentActions += ` <a class="toggle-replies-btn btn row no-src" data-comment-id="${comment.id}" data-amount-of-replies="${comment.amountOfReplies}">View Replies</a>`;
                        }
                    }

                    if (derivingFromId == 0) {
                        commentActions += `<div class="sub-replies-container" id="sub-replies-container-${comment.id}" style="display: none;"></div>`;
                    }

                    var repliesHtml = `
                        <div class="profile-column col-1 mx-2 p-0">
                            <a class="" href="Home/Account?username=${currReplyUsername}">
                                ${imgTag}
                            </a>
                        </div>
                        <div class="comment-column col mx-2 p-0">
                            <div class="comment" data-comment-id=${comment.id} data-comment-username=${comment.appUser.userName}>
                                <div class="comment-header rounded-top">
                                    <div class="comment-username font-weight-bold">
                                        <a class="custom-username-com-link" href="Home/Account?username=${currReplyUsername}">
                                            @${comment.appUser.userName}
                                        </a>
                                    </div>
                                    <div class="comment-time text-muted">${getTimeDisplay(comment.dateOfCreation)}</div>
                                </div>
                                <div class="comment-content rounded-bottom">
                                    ${commentContent}
                                    <div class="row">${likeCount}</div>
                                </div>
                                ${commentActions}
                            </div>
                        </div>`;

                    var tempDiv = document.createElement('div');
                    tempDiv.setAttribute('class', 'comment-container row pt-2');
                    tempDiv.setAttribute('data-comment-id', comment.id);
                    tempDiv.setAttribute('data-deriving-from', derivingFromId);
                    tempDiv.innerHTML = repliesHtml;
                    fragment.appendChild(tempDiv); 
                });
            }

            if (posts.length < 20) {
                if (derivingFromId > 0) {
                    $('.load-more-replies').hide();
                } else {
                    $('.load-more-comments').hide();
                }
            }

            if (derivingFromId === 0) {
                var containerAppenderId = `comment-container-${postAttrId}`;
                var containerAppender = $('#' + containerAppenderId);
                containerAppender.append(fragment); 
            } else {
                var repliesContainer = buttonEvent.next();

                if (repliesContainer.length && repliesContainer.hasClass('sub-replies-container')) {
                    repliesContainer.append(fragment); 
                }
                else {
                    var parentContainer = buttonEvent.closest('.sub-replies-container');

                    var targetElement = parentContainer.find(`.comment-container[data-comment-id="${derivingFromId}"]`);
                    targetElement.after(fragment); 
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

function likecom(button, Id) {
    var card = button.closest(".comment");
    var cardId = card.dataset.id;
    var count = parseInt(button.dataset.count) || 0;
    var likedComments = JSON.parse(sessionStorage.getItem("likedComments")) || {};

    var heartIcon = button.querySelector(".com-btn");

    if (heartIcon.classList.contains("liked")) {
        count--;
        button.dataset.count = count;
        button.innerHTML = `<i class="fa-regular fa-heart fa-xl com-btn" style="color: #000;"></i>`;
        likeCommentClick(Id);
        delete likedComments[cardId];
    } else {
        count++;
        button.dataset.count = count;
        button.innerHTML = `<i class="fa-solid fa-heart liked fa-xl com-btn" style="color: #ff0000;"></i>`;
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

function toggleComments(element, itemId, buttonEvent) {
    const card = element.closest('.card');
    const cardId = `card-overlay-${itemId}`;
    var postId = $(card).data('post-id');

    loadMoreComments(0, postId, buttonEvent);

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


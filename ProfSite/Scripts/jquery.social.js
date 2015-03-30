


(function ($) {
    $.extend($.ui, { TwitterTimeline: {} });

    function TwitterTimeline(options) {
        this.timelineUrl = null;
        this.replyUrl = null;
        this.retweetUrl = null;
        this.timelineElement = null;
    }

    $.extend(TwitterTimeline.prototype, {
        init: function (options, control) {
            this.widget = control;

            var defaults = {
                timelineUrl: '/twitter/timeline',
                replyUrl: '/twitter/reply',
                retweetUrl: '/twitter/retweet'
            };

            var allOptions = $.extend(defaults, options);

            this.timelineUrl = allOptions.timelineUrl;
            this.replyUrl = allOptions.replyUrl;
            this.retweetUrl = allOptions.retweetUrl;
            this.loadTimeline = allOptions.loadTimeline != undefined ? allOptions.loadTimeline : true;

            this.timelineElement = control;
            this.callback = options.callback != undefined ? options.callback : null;

            if (this.loadTimeline === true) {
                this._timeline();
            } else {
                this._bindEvents(this.timelineElement, this);
            }
            return this;
        },
        _bindPostToTwitter: function (self) {
            $("#post-to-twitter").socialPostDialog({"dialogType":"twitter"});
        },
        _timeline: function () {
            var self = this;
            var loading = $("#twitter-loading");
            loading.show();
            self.timelineElement.hide();
            $.ajax({
                url: self.timelineUrl,
                cache: false,
                type: "GET",
                success: function (data) {
                    self.timelineElement.html(data).show();
                    self.timelineElement = $(".twitter-timeline");

                    self._bindEvents(self.timelineElement, self);
                    self._nextPage(self);
                    self._bindPostToTwitter(self);
                    self._bindRefresh(self);
                    if (self.callback) {
                        self.callback.call(this);
                    }
                    loading.hide();
                }
            });
        },
        _bindRefresh: function (self) {
            $("#refreshTwitter").click(function () {
                self._timeline();
            });
        },
        _bindEvents: function (element, self) {
            self._reply(element.find("li"), self);
            self._retweet(element.find(".btn-twitter-retweet"), self);
        },
        _nextPage: function (self) {

            var nextLink = $("#twitter-more"), self = this;

            nextLink.click(function (event) {
                event.preventDefault();

                var anchor = $(this);

                var href = anchor.attr("href");
                var loadingClone = $(".tweetLoading").clone();
                $(".twitter-timeline").append(loadingClone);
                loadingClone.show();
                anchor.hide();
                $.ajax({
                    url: href,
                    type: "GET",
                    dataType: "HTML",
                    success: function (data) {
                        var source = "<div>" + data + "</div>";

                        var newPageLink = $(source).find(".next-page");

                        if ($(newPageLink).length >= 1) {
                            var nextPageHref = newPageLink.attr("href");
                            $("#twitter-more").attr("href", nextPageHref);
                            anchor.show();
                        } else {

                        }

                        var newElements = $(source).find("li.social-post").appendTo(".twitter-timeline");
                        loadingClone.remove();
                        //self._bindEvents(newElements, self);
                        self.timelineElement = $(".twitter-timeline");
                        self._bindEvents(self.timelineElement, self);
                    }
                });

            });
        },
        _retweet: function (element, self) {

            element.click(function (event) {
                event.preventDefault();
                var elementSelf = $(this);

                if (elementSelf.hasClass("retweeted")) {
                    return;
                }

                var postId = $(this).attr("href");

                $.ajax({
                    url: self.retweetUrl,
                    data: "postId=" + postId,
                    type: "POST",
                    dataType: "json",
                    cache: false,
                    success: function () {
                        elementSelf.addClass("retweeted");
                    }
                });
            });
        },
        _reply: function (element, self) {
            $(element).find(".btn-twitter-reply").click(function (event) {
                event.preventDefault();
                var postId = $(this).attr("href");

                var postreplydiv = $("#post-reply-" + postId).toggleClass("collapsed");

                $("#post-reply-button-" + postId).click(function () {
                    var textreply = $("#post-reply-text-" + postId);
                    var text = textreply.val();
                    var tweetLoadingClone = $(".tweetLoading").clone();
                    $("ul.twitter").prepend(tweetLoadingClone);
                    tweetLoadingClone.show();
                    $.ajax({
                        url: self.replyUrl,
                        data: "postId=" + postId + "&replyText=" + text,
                        type: "POST",
                        cache: false,
                        success: function (data) {
                            postreplydiv.slideUp();
                            textreply.val('');
                            tweetLoadingClone.remove();
                            var newElement = $("ul.twitter").prepend(data);
                            self._bindEvents(newElement, self);
                        }
                    });
                });
            });
        }
    });
    $.fn.twitterTimeline = function (options) {
        var control = new TwitterTimeline();
        control.init(options, this);
        return control;
    };
})(jQuery);

(function ($) {
    $.extend($.ui, { FacebookTimeline: {} });

    function FacebookTimeline() {
        this.timelineUrl = null;
        this.timelinePlaceHolder = null;
        this.timelineContainer = null;
        this._shareDialog = null;
    }

    $.extend(FacebookTimeline.prototype, {
        init: function (options, control) {
            this.widget = control;

            var defaults = {
                timelineUrl: '/facebook/timeline',
            };

            var allOptions = $.extend(defaults, options);

            this.timelineUrl = allOptions.timelineUrl;
            this.timelinePlaceHolder = control;

            this._timeline();

            return this;
        },
        _bindPostToFacebook: function (self) {
            $("#post-to-facebook").socialPostDialog({});

        },
        
        _timeline: function () {
            var self = this;
            var loading = $("#fb-loading");

            loading.show();
            self.timelinePlaceHolder.hide();
            $.ajax({
                url: self.timelineUrl,
                type: "GET",
                cache: false,
                success: function (data) {
                    self.timelinePlaceHolder.html(data).show();
                    self._bindEvents($(".facebook-timeline"), self);
                    self._nextPage(self);
                    self._bindPostToFacebook(self);

                    loading.hide();
                }
            });
        },
        _nextPage: function (self) {

            var nextLink = $("#facebook-more");

            nextLink.click(function (event) {
                event.preventDefault();

                var anchor = $(this);

                var href = anchor.attr("href");
                var fbLoadingClone = $(".fbLoading").clone();
                $(".facebook-timeline").append(fbLoadingClone);
                fbLoadingClone.show();
                anchor.hide();
                $.ajax({
                    url: href,
                    type: "GET",
                    dataType: "HTML",
                    success: function (data) {
                        var source = "<div>" + data + "</div>";

                        var newPageLink = $(source).find(".next-page");
                        if ($(newPageLink).length >= 1) {
                            var nextPageHref = newPageLink.attr("href");
                            $("#facebook-more").attr("href", nextPageHref);
                            anchor.show();
                        } else {
                            
                        }

                        var newElements = $(source).find("li.social-post").appendTo(".facebook-timeline");
                        fbLoadingClone.remove();
                        self._bindEvents(newElements, self);
                    }
                });

            });


        },
        _bindEvents: function (element, self) {
            self._like(element.find(".fb-like-post"), self);
            self._comment(element.find(".fb-comment"), self);
        },
        _like: function (element, self) {

            element.click(function (event) {
                event.preventDefault();

                var elementSelf = $(this);

                if (elementSelf.hasClass("liked")) {
                    return;
                }

                var postId = $(this).attr("href");

                $.ajax({
                    url: "/facebook/likepost",
                    data: "postId=" + postId,
                    type: "POST",
                    dataType: "json",
                    cache: false,
                    success: function (data) {
                        elementSelf.addClass("liked");
                        var text = elementSelf.text();
                        var newText = elementSelf.data("unliketext");
                        elementSelf.text(newText);
                    }
                });
            });
        },
        _comment: function (element, self) {

            element.submit(function (event) {
                event.preventDefault();

                var postId = $(this).attr("action");
                var input = $(this).find("input");
                var message = input.val();

                $.ajax({
                    url: "/facebook/commentpost",
                    data: "message=" + message + "&postId=" + postId,
                    type: "POST",
                    cache: false,
                    success: function (data) {
                        var newElement = $(data).appendTo($(".fb-comment-" + postId));
                        input.val('');
                        self._bindEvents(newElement, self);
                    }
                });
            });
        }

    });
    $.fn.facebookTimeline = function (options) {
        var control = new FacebookTimeline();
        control.init(options, this);
        return control;
    };
})(jQuery);


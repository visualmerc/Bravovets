'use strict';

angular.module('chartsApp')
.factory('mouseCapture', ['$rootScope', function ($rootScope)
{
    var $element = document,
    mouseCaptureConfig = null,
    mouseMove = function (evt) {
        if (mouseCaptureConfig && mouseCaptureConfig.mouseMove) {
            mouseCaptureConfig.mouseMove(evt);
            $rootScope.$digest();
        }
    },
    mouseUp = function (evt) {
        if (mouseCaptureConfig && mouseCaptureConfig.mouseUp) {
            mouseCaptureConfig.mouseUp(evt);
            $rootScope.$digest();
        }
    };
    return {
        registerElement: function (element)
        {
            $element = element;
        },
        acquire: function (evt, config)
        {
            this.release();
            mouseCaptureConfig = config;
            $element.on('mousemove', mouseMove);
            $element.on('mouseup', mouseUp);
            $element.on('mouseout', mouseUp);
            $element.bind('touchmove', mouseMove);
            $element.bind('touchend', mouseUp);
        },
        release: function ()
        {
            if (mouseCaptureConfig)
            {
                if (mouseCaptureConfig.released)
                {
                    mouseCaptureConfig.released();
                }

                mouseCaptureConfig = null;
                $element.unbind('mousemove', mouseMove);
                $element.unbind('mouseup', mouseUp);
                $element.unbind('touchmove', mouseMove);
                $element.unbind('touchend', mouseUp);
            }
        },
    };
}])
.directive('mouseCapture', function ()
{
    return {
        restrict: 'A',
        controller: ['$scope', '$element', '$attrs', 'mouseCapture', function ($scope, $element, $attrs, mouseCapture)
        {
            mouseCapture.registerElement($element);
        }],
    };
})
.directive('imgRotator', ['mouseCapture', function (mouseCapture)
{
    return {
        restrict: 'E',
        template: '<div class="mouseCapture-box {{CSSclass}}" ng-style="getStyles(draggable)"></div>',
        replace: true,
        scope: {
            src: '@',
            class: '@',
            draggable: '&'
        },
        link: function ($scope, $element, $attrs) {
            $element.on('mousedown', function (evt){ $scope.mousedown(evt); });
            $element.on('touchstart', function (evt){ $scope.mousedown(evt); });

            $scope.getStyles = function(draggable)
            {
                return {
                    height: draggable.height,
                    width: draggable.width,
                    background: 'url(' + draggable.img + ') no-repeat'
                };
            };
        },
        controller: ['$scope', '$element', '$attrs', 'mouseCapture', 'dragging', function ($scope, $element, $attrs, mouseCapture, dragging)
        {
            $scope.draggable = {
                x: 0,
                y: 0,
                distance: '0',
                prevVector: {
                    x: 0,
                    y: 0
                },
                CSSclass: $scope.class,
                img: $scope.src,
                height: $attrs.height + 'px',
                width: $attrs.width + 'px',
                index: 1,
                Ypos: 0,
                Xpos: 0,
                sprites: $attrs.sprites - 1
            };

            $scope.mousedown = function (event) {
                var startX = event.pageX,
                    startY = event.pageY,
                    orientation = ($attrs.orientation ? $attrs.orientation : 'vertical');

                dragging.startDrag(event, {
                    dragStarted: function () {
                        $scope.draggable.xstart = startX;
                        $scope.draggable.ystart = startY;
                    },
                    dragging: function (x, y, evt) {
                        $scope.draggable.prevVector.x = $scope.draggable.x;
                        $scope.draggable.prevVector.y = $scope.draggable.y;
                        $scope.draggable.x = x;
                        $scope.draggable.y = y;
                        $scope.draggable.distance = Math.abs($scope.draggable.prevVector.x - x);
                        if($scope.draggable.distance > 0)
                        {
                            $scope.draggable.direction = ($scope.draggable.prevVector.x > x) ? 'left' : 'right';
                            if($scope.draggable.direction === 'right')
                            {
                                $scope.draggable.index++;
                                if($scope.draggable.index > $scope.draggable.sprites)
                                {
                                    $scope.draggable.index = 1;
                                }
                            } else {
                                $scope.draggable.index--;
                                if($scope.draggable.index < 1)
                                {
                                    $scope.draggable.index = $scope.draggable.sprites;
                                }
                            }
                            var Ypos = $scope.draggable.index * $attrs.height;
                            $scope.draggable.Ypos = Ypos - (Ypos*2);

                            var Xpos = $scope.draggable.index * $attrs.width;
                            $scope.draggable.Xpos = Xpos - (Xpos*2);

                            if(orientation === 'vertical')
                            {
                                $element.css('background-position', '0px ' + $scope.draggable.Ypos + 'px');
                            }
                            else if(orientation === 'horizontal')
                            {
                                $element.css('background-position', $scope.draggable.Xpos + 'px 0px');
                            }
                        }
                    }
                });
            };
        }]
    };
}]);

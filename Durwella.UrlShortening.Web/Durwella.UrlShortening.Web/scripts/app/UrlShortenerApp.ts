/// <reference path="../typings/angularjs/angular.d.ts" />

/**
 * The main app module.
 *
 * @type {angular.Module}
 */
module UrlShortenerApp {
    "use strict";

    interface IUrlShortenerScope extends ng.IScope {
        longUrl: string;
        customPath: string;
        shortenedUrl: string;
        errorMessage: string;
        waiting: boolean;
        submitLongUrl: Function;
    }

    class UrlShortenerCtrl {

        // $inject annotation.
        // It provides $injector with information about dependencies to be injected into constructor
        // it is better to have it close to the constructor, because the parameters must match in count and type.
        // See http://docs.angularjs.org/guide/di
        public static $inject = [
            "$scope",
            "$http",
            "$location"
        ];

        // dependencies are injected via AngularJS $injector
        // controller's name is registered in Application.ts and specified from ng-controller attribute in index.html
        constructor(
            private $scope: IUrlShortenerScope,
            private $http: ng.IHttpService,
            private $location: ng.ILocationService
            ) {
            $scope.submitLongUrl = function() {
                if ($scope.longUrl) {
                    $scope.shortenedUrl = null;
                    $scope.errorMessage = null;
                    $scope.waiting = true;
                    $http.post('/shorten', { Url: $scope.longUrl, CustomPath: $scope.customPath })
                        .success(function (response: any) {
                            $scope.shortenedUrl = response.Shortened;
                            $scope.waiting = false;
                        })
                        .error(function (response: any) {
                            console.log('Error: ', response);
                            var message = "Unknown Error";
                            if (response.hasOwnProperty('ResponseStatus') && response.ResponseStatus.hasOwnProperty('Message'))
                                message = response.ResponseStatus.Message;
                            $scope.errorMessage = message;
                            $scope.waiting = false;
                        });
                }
            }
        }
    }

    angular.module('urlShortenerApp', [])
        .controller('urlShortenerCtrl', UrlShortenerCtrl);
}

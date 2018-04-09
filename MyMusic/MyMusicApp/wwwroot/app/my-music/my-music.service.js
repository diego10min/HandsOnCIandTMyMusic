'use strict';

angular.module('myMusic')
    .factory('myMusicService', ['$http', '$q', function ($http, $q) {

        return {
            getLibrary: getLibrary,
            getUserPlaylist: getUserPlaylist,
            addToPlaylist: addToPlaylist,
            removeFromPlaylist: removeFromPlaylist,
        }

        function getLibrary(filter) {
            return $http.get('http://localhost:5001/api/musica/' + filter)
                .then(function (response) {
                    return response.data;
                });
        }

        function getUserPlaylist(filter) {
            return $http.get('http://localhost:5002/api/playlist/' + filter)
                .then(function (response) {
                    return response.data || {};
                });
        }

        function addToPlaylist(playlistId, musica) {
            return $http.put('http://localhost:5002/api/playlist/addmusica/' + playlistId, musica)
                .then(function (response) {
                    return response.data;
                });
        }

        function removeFromPlaylist(playlistId, musica) {
            return $http.put('http://localhost:5002/api/playlist/removemusica/' + playlistId, musica)
                .then(function (response) {
                    return response.data;
                });
        }
    }]);
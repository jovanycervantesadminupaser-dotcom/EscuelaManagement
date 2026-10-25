const CACHE_NAME = 'upaser-cache-v1';
const OFFLINE_URL = 'offline.html';

self.addEventListener('install', function (event) {
    event.waitUntil(
        caches.open(CACHE_NAME).then(function (cache) {
            return cache.addAll([
                OFFLINE_URL,
                'icon-192.png',
                'icon-512.png'
            ]);
        })
    );
});

self.addEventListener('fetch', function (event) {
    if (event.request.mode === 'navigate' || (event.request.method === 'GET' && event.request.headers.get('accept').includes('text/html'))) {
        event.respondWith(
            fetch(event.request.url).catch(error => {
                // Si la red falla, devolvemos la página offline
                return caches.match(OFFLINE_URL);
            })
        );
    }
});
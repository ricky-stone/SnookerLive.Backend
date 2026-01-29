/* global use, db */

use('Matches');

// Change this to the collection you want to drop
const collectionName = 'all';

const result = db.getCollection(collectionName).drop();
console.log(`Dropped collection '${collectionName}': ${result}`);
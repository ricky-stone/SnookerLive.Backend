use('Matches');

const filter = { Status: 0 };

const count = db.getCollection('all').countDocuments(filter);

const results = db.getCollection('all')
  .find(filter)
  .limit(50)
  .toArray();

({ count, results });
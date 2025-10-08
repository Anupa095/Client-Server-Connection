const admin = require('firebase-admin');
const express = require('express');
const bodyParser = require('body-parser');

admin.initializeApp({
  credential: admin.credential.cert({
    // Your Firebase Admin SDK credentials
    projectId: '<PROJECT_ID>',
    privateKey: '<PRIVATE_KEY>',
    clientEmail: '<CLIENT_EMAIL>',
  }),
});

const app = express();
app.use(bodyParser.json());

app.post('/login', async (req, res) => {
  const { username, password } = req.body;
  
  // Validate username and password (replace with your own logic)
  if (username === 'admin' && password === 'admin123') {
    try {
      // Create a custom token
      const customToken = await admin.auth().createCustomToken(username);
      res.json({ token: customToken });
    } catch (error) {
      res.status(500).send('Error creating custom token');
    }
  } else {
    res.status(401).send('Invalid credentials');
  }
});

app.listen(3001, () => console.log('Server running on port 3001'));

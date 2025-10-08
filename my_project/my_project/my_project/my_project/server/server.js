const express = require('express');
const mongoose = require('mongoose');
const bodyParser = require('body-parser');
const cors = require('cors');

const app = express();
const PORT = process.env.PORT || 5000;

// Middleware
app.use(cors());
app.use(bodyParser.json());

// MongoDB connection
mongoose.connect('mongodb://localhost:27017/employeeDB', {
  useNewUrlParser: true,
  useUnifiedTopology: true,
})
.then(() => console.log('MongoDB connected'))
.catch(err => console.error('MongoDB connection error:', err));

// Employee model
const EmployeeSchema = new mongoose.Schema({
  firstName: String,
  lastName: String,
  email: String,
  contactNumber: String,
  role: String
});

const Employee = mongoose.model('Employee', EmployeeSchema);

// Routes
app.post('/api/employees', async (req, res) => {
  try {
    const { firstName, lastName, email, contactNumber, role } = req.body;
    const newEmployee = new Employee({ firstName, lastName, email, contactNumber, role });
    await newEmployee.save();
    res.status(201).json({ message: 'Employee registered successfully', employee: newEmployee });
  } catch (error) {
    res.status(500).json({ error: 'Failed to register employee', details: error.message });
  }
});

app.listen(PORT, () => console.log(`Server running on port ${PORT}`));

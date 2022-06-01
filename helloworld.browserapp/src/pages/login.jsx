import React, { Component } from 'react';
import './Login.css';
import 'bootstrap/dist/css/bootstrap.css';
import { Form, Button } from 'react-bootstrap';
import wave from '../images/wave.png';
import email from '../images/email.png';
import password from '../images/padlock.png';
import logo from '../images/logo.png';

import InputField from '../components/inputfield';
import '../Colors.css';

class Login extends Component {
    onClick() {
        
    }

    render() {
        return (
            <div className='Login-component'>
                <img src={wave} alt='wave' className='background'/>
                <Form className='Login-form'>
                    <h2 className='BrightOrange' style={{ paddingBottom: '10%', float: 'left' }}>Welcome</h2>
                    <img src={logo} className="logo" alt="" />
                    <Form.Group className="mb-3" controlId="formBasicEmail">
                        <InputField type='text' placeholder="Email" icon={email}/>
                    </Form.Group>

                    <Form.Group className="mb-3" controlId="formBasicPassword">
                        <InputField type='password' placeholder='Password' icon={password} />
                    </Form.Group>

                    <Form.Group className="mb-3" controlId="formBasicCheckbox">
                        <Form.Check type="checkbox" label="Remember me?" />
                    </Form.Group>
                    <div className="text-center submit">
                        <Button variant="primary" type="submit" onClick={this.onClick}>
                            Submit
                        </Button>
                    </div>
                </Form>
            </div>
        );
    }
}

export default Login;
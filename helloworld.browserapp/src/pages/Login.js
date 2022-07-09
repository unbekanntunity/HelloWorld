import React, { Component } from 'react';
import wave from '../images/wave.png';
import email from '../images/email.png';
import password from '../images/padlock.png';
import logo from '../images/logo.png';

import InputField from '../components/InputField/InputField';

import { sendJSONRequest } from '../requestFuncs';
import { Link } from 'react-router-dom';
import { Form, Button } from 'react-bootstrap';

import './Login.css';

class Login extends Component {
    state = {
        email: "",
        password: "",

        formClassNames: 'login-form animated-fadeIn'
    };

    handleSubmit = async () => {
        await sendJSONRequest("POST", "/identity/login", {
            email: this.state.email,
            password: this.state.password
        })
        .then(data => {
            if (data.errors === undefined) {
                this.setState({ formClassNames: 'login-form animated-fadeOut' });
                setTimeout(() => this.props.onLoginSuccess(data.token, data.refreshToken), 1000)
            }
            else {
                this.setState({
                    message: data.errors,
                    showToast: true
                });
            }
        },
        error => {
            this.props.onError(error);
        });
    }

    handleInput = (event) => {
        this.setState({ [event.target.name]: event.target.value });
    }
    
    handleGoToRegistration = () => {
        this.setState({ formClassNames: 'login-form animated-fadeOut' });
        setTimeout(this.props.onGoToRegistration, 1000)
    }

    handleClose = () => {
        this.setState({ showToast: this.state.showToast });
    }

    render() {
        return (
            <div>
                <div className='login-component'>
                    <img src={wave} alt='wave' style={{ width: "100%" }} />
                    <Form className={this.state.formClassNames}>
                        <h2 style={{ paddingBottom: '10%', float: 'left', color: '#FF5C00' }}>Welcome back</h2>
                        <img src={logo} className="logo" alt="" />

                        <Form.Group className="mb-3" controlId="formBasicEmail">
                            <InputField propName="email" type='text' placeholder="Email" icon={email} iconSize='8%' style={{ innerHeight: 100 }} onChange={this.handleInput} />
                        </Form.Group>
                        <Form.Group className="mb-3" controlId="formBasicPassword">
                            <InputField propName="password" type='password' placeholder='Password' icon={password} iconSize='8%' onChange={this.handleInput} />
                        </Form.Group>
                        <Form.Group className="mb-3" controlId="formBasicCheckbox" style={{marginLeft: '10%'}}>
                            <Form.Check type="checkbox" label="Remember me?" />
                        </Form.Group>
                        <div className="text-center submit" style={{ marginTop: '45px' }}>
                            <Button variant="primary" onClick={this.handleSubmit}>Sign in</Button>
                            <p style={{ marginTop: '25px' }}>New User? <Link to="/registration">Sign up</Link> here</p>
                        </div>
                   </Form>
                </div>
            </div>
        );
    }
}

export default Login;
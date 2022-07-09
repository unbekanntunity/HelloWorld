import React, { Component } from 'react';

import wave from '../images/wave.png';
import email from '../images/email.png';
import password from '../images/padlock.png';
import logo from '../images/logo.png';
import username from '../images/id-card.png'

import InputField from '../components/InputField/InputField';

import { sendJSONRequest, sendRequest } from '../requestFuncs';
import { Link } from 'react-router-dom';
import { Form, Button } from 'react-bootstrap';

import './Registration.css';

class Registration extends Component {
    state = {
        email: "",
        username: "",
        password: "",
        repeatedPassword: "",

       
        formClassNames: 'registration-form animated-fadeIn'
    };

    handleSubmit = async () => {
        if (this.state.password !== this.state.passwordRepeat) {
            this.props.onError({
                message: "Passwords are not equal"
            })
            return;
        }

        await sendJSONRequest("Post", "/identity/register", {
            username: this.state.username,
            email: this.state.email,
            password: this.state.password,
        })
            .then(data => {
                if (data.errors === undefined) {
                    this.setState({ formClassNames: 'registration-form animated-fadeOut' });
                    setTimeout(this.props.onRegistrationSuccess, 1000)
                }
                else {
                    this.props.onError(data.errors);
                }
            }, error => {
                this.props.onError(error);
            });
    }

    handleInput = (event) => {
        this.setState({ [event.target.name]: event.target.value });
    }

    handleClose = () => {
        this.setState({ showDialog: false });
    }

    handleGoToLogin = () => {
        this.setState({ formClassNames: 'registration-form animated-fadeOut' });
        setTimeout(this.props.onGoToLogin(), 1000)
    }

    render() {
        return (
            <div>
                <div className='registration-component'>
                    <img src={wave} alt='wave' style={{ width:"100%" }} />
                    <Form id="form" className='registration-form'>
                        <h2 style={{ paddingBottom: '10%', float: 'left', color: '#FF5C00' }}>Welcome</h2>
                        <img src={logo} className="logo" alt="" />
                        <Form.Group className="mb-3" controlId="formBasicUsername">
                            <InputField name="username" type='text' placeholder="Username" icon={username} iconSize='8%' />
                        </Form.Group>
                        <Form.Group className="mb-3" controlId="formBasicEmail">
                            <InputField name="email" type='text' placeholder="Email" icon={email} iconSize='8%'/>
                        </Form.Group>
                        <Form.Group className="mb-3" controlId="formBasicPassword">
                            <InputField name="password" type='password' placeholder='Password' icon={password} iconSize='8%'/>
                        </Form.Group>
                        <Form.Group className="mb-3" controlId="formBasicPasswordRepeat">
                            <InputField name="repeatedPassword" type='password' placeholder='Repeated password' icon={password} iconSize='8%' />
                        </Form.Group>
                        <Form.Group className="mb-3" controlId="formBasicCheckbox" style={{marginLeft: '10%'}}>
                            <Form.Check type="checkbox" label="Remember me?" />
                        </Form.Group>
                        <div className="text-center submit" style={{ marginTop: '40px' }}>
                            <Button variant="primary" onClick={this.handleSubmit}>Sign up</Button>
                            <p style={{ marginTop: '25px' }}>New User? <Link to="/">Sign in</Link> here</p>
                        </div>
                   </Form>
                </div>
            </div>
        );
    }
}

export default Registration;
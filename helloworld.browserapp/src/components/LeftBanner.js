import React, { Component } from 'react';

import './LeftBanner.css';

class LeftBanner extends Component {
    render() {
        return (
            <div className="left-banner">
                <p className="left-banner-text">{this.props.text}</p>
            </div>
        );
    }
}

export default LeftBanner;
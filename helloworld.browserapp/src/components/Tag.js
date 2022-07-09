import React, { Component } from 'react';

import "./Tag.css";

class Tag extends Component {
    getColor = () => {
        if (!this.props.name) {
            return;
        }

        if (this.props.name.indexOf("Support") !== -1) {
            return "red";
        }
        else {
            return "#0079D3";
        }
    }

    render() {
        return (
            <div className="tag-container" style={{
                backgroundColor: this.getColor(),
                fontSize: this.props.fontSize ?? '10px'
            }}>
                <p className="tag-text">{this.props.name}</p>
                {
                    this.props.removable &&
                    <img src={this.props.removeIcon} alt="" style={{
                        marginLeft: "5px",
                        width: this.props.iconSize,
                        height: this.props.iconSize
                    }} onClick={() => this.props.onRemove(this.props.propKey)} />
                }
            </div>
        );
    }
}

export default Tag;
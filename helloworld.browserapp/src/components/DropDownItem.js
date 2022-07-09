import React, { Component } from 'react';

import './DropDownItem.css';

class DropDownItem extends Component {
	render() {
		return (
			<div className="drop-container" style={{ borderBottom: this.props.lastItem ? "none" : "1px solid #5A5A5A" }}
				onClick={this.props.onClick}>
				<img className="drop-icon" src={this.props.icon} width={this.props.iconSize} height={this.props.iconSize} alt="" />
				<p className="drop-text" style={{ color: this.props.textColor }}>{this.props.text}</p>
			</div>
		);
	}
}

export default DropDownItem;
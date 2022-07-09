import React, { Component } from 'react';

import './MultiInputField.css';

class MultiInputField extends Component {
	state = {
		letters: this.props.placeholder.lenght ?? 0,
		counterClasses: 'letters'
	}

	handleChange = (event) => {
		const numberLetters = event.target.value.length;

		if (numberLetters > this.props.maxLetters) {
			this.setState({
				letters: numberLetters,
				counterClasses: 'letters red'
			});
		}
		else {
			this.setState({
				letters: numberLetters,
				counterClasses: 'letters'
			});
		}

		if (this.props.onChange !== undefined) {
			this.props.onChange(event);
        }
	}

	render() {
		return (
			<div className="multi-container" style={{ zIndex: this.props.zIndex ?? 1 }}>
				<textarea className="textbox" style={{
					height: this.props.height ?? '100%',
					width: this.props.width ?? '100%'
				}} placeholder={this.props.placeholder} onChangeCapture={this.handleChangeCapture} onChange={this.handleChange} />
				<p className={this.state.counterClasses}>{this.state.letters}/{this.props.maxLetters}</p>
			</div>
		);
	}
}

export default MultiInputField
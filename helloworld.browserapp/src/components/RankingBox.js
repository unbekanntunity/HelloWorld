import React, { Component } from 'react';

import Tag from './Tag';

import "./RankingBox.css";

class RankingBox extends Component {
	getColor(place) {
		switch (place) {
			case 1:
				return "#FFB30B";
			case 2:
				return "#ADADAD";
			default:
				return "black";
		}
	}

	render() {
		const itemsArray = Array.from(this.props.items);

		return (
			<div className="ranking-box-container" style={{ width: this.props.width, height: this.props.height }}>
				<div className="ranking-box-header">
					<p>{this.props.title}</p>
				</div>
				<div className="scroll">
					{
						this.props.items !== undefined &&
						itemsArray.map((item, index) =>
							<div key={index} className={index !== (itemsArray.length - 1) ? "ranking-box-item" : "ranking-box-item last"}
								onClick={() => this.onItemClicked(item, index)}>
								<div style={{ width: "90%" }}>
									<p className="ranking-box-item-name">{item.name}</p>
									<div className="ranking-box-item-tags">
										{
											item.tags &&
											Array.from(item.tags).slice(0, 3).map((tag, index) => <Tag key={`key-${index}`} name={tag} /> )
										}
									</div>
								</div>
								<p style={{
									color: this.getColor(index + 1),
									fontWeight: 'bold',
									marginRight: "20px"
									}}>{index + 1}</p>
							</div>
						)
					}
				</div>
			</div>
		);
	}
}

export default RankingBox;